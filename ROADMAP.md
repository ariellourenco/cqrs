# Roadmap

Modernizing the [CQRS Journey](https://github.com/microsoftarchive/cqrs-journey) conference-management sample onto current .NET,
targeting an [eShop](https://github.com/dotnet/eShop)-style architecture: .NET Aspire, microservices, Minimal APIs, and an
integration-event bus.

## How to use this file

This is the master plan, built for **short, self-contained sessions**. Each milestone is a shippable increment; each task is meant to
be doable in one sitting. Tick boxes as you go (`- [x]`). Pick the next unchecked task in the lowest open milestone — order matters,
later milestones assume earlier ones. When a milestone is fully checked, cut a git tag or a release.

## Architectural decisions

- **Hybrid CQRS.** Event-sourced *write* side for the interesting contexts (Registration/Seats, Orders) and pragmatic EF Core CRUD for
  the rest (Conference Management, Payments). This showcases both styles without event-sourcing *everything*.
- **Event store:** [Marten](https://martendb.io) on PostgreSQL for the event-sourced contexts (event store + async projections +
  Aspire integration out of the box).
- **Relational store:** EF Core on PostgreSQL for the CRUD contexts.
- **Messaging:** RabbitMQ, with the existing `IEventBus` abstraction implemented the eShop way (hand-rolled publisher/subscriber over
  integration events; not MassTransit — closer to the reference architecture).
- **Orchestration:** .NET Aspire AppHost is the single entry point for local dev.

### Bounded contexts (from cqrs-journey → services here)

| Context                           | Style                | Store              | Notes                                                                    |
|-----------------------------------|----------------------|--------------------|--------------------------------------------------------------------------|
| Conference Management             | CRUD                 | EF Core            | Create/publish conferences, define seat types. Emits integration events. |
| Registration (Seats Availability) | Event Sourced        | Marten             | The `SeatsAvailability` aggregate you already have.                      |
| Orders & Registration Process     | Event Sourced + Saga | Marten             | Order aggregate + registration process manager (the CQRS showpiece).     |
| Payments                          | CRUD                 | EF Core            | Simplest service; good "second service" warm-up.                         |
| Conference (read side)            | Read model           | Marten projections | Priced-order / conference-view projections consumed by the UI.           |

---

## Milestone 0 — Housekeeping (tiny, do first)

Fix the loose ends before adding anything new.

- [ ] Create `src/Infrastructure/EventBus/EventBus/EventBus.csproj` for the orphaned `IEventBus.cs` and add it to `Conference.slnx`.
- [ ] Decide and document the canonical namespace/folder convention (currently `CQRSJourney.Registration`,
      `CQRSJourney.Infrastructure.EventBus.*`). Note it in `copilot-instructions.md`.
- [ ] Add `ROADMAP.md` to the `Solution Items` folder in `Conference.slnx`.
- [ ] Add a top-level architecture diagram (Mermaid) to `README.md` or `docs/architecture.md`.

## Milestone 1 — Aspire orchestration + first real endpoint

Goal: `dotnet run` on the AppHost brings up the Aspire dashboard with Registration.Api showing health + telemetry, and one working
HTTP endpoint. **This is the most motivating milestone — do it early.**

- [ ] Add `src/Aspire/CQRSJourney.ServiceDefaults` (OpenTelemetry, health checks, service discovery, resilience handler). Reference it
      from `Registration.Api`.
- [ ] Add `src/Aspire/CQRSJourney.AppHost` (Aspire host project). Register `Registration.Api`.
- [ ] Wire `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()` into `Registration.Api`.
- [ ] Add a Scalar or built-in OpenAPI UI for exploring the API in dev.
- [ ] Add one real endpoint group: `GET /seats-availability/{id}` returning the aggregate state (in-memory for now — persistence comes
      next). Use typed results (`Results<Ok<T>, NotFound>`).
- [ ] Add `dotnet run --project AppHost` instructions to `README.md`.
- [ ] Verify the dashboard shows traces/logs for a request end-to-end.

## Milestone 2 — Persistence for the event-sourced write side

Goal: `SeatsAvailability` is stored as an event stream and rebuilt from events.

- [ ] Add a PostgreSQL resource to the AppHost (`builder.AddPostgres(...).AddDatabase(...)`).
- [ ] Add Marten to `Registration.Api` and configure it against the Aspire Postgres connection.
- [ ] Implement `IRepository<SeatsAvailability>` + `IUnitOfWork` over Marten's event store (append `AddEvent` events on save,
      `AggregateStreamAsync` to load).
- [ ] Add command endpoints: `POST /seats-availability` (add seats), `POST .../reservations` (make reservation),
      `POST .../reservations/{id}/commit`, `DELETE .../reservations/{id}`.
- [ ] Add an integration test using `Aspire.Hosting.Testing` that boots Postgres and round-trips an aggregate.

## Milestone 3 — Integration-event bus

Goal: services communicate via RabbitMQ integration events; the abstraction is real.

- [ ] Add a RabbitMQ resource to the AppHost.
- [ ] Flesh out the `EventBus` project: `IntegrationEvent` base type, `IIntegrationEventHandler<T>`, an `EventBusRabbitMQ`
      implementation of `IEventBus`, and an `AddEventBus` extension.
- [ ] Publish `SeatsReserved` / `AvailableSeatsChanged` as integration events after the aggregate commits (outbox-lite: publish inside
      the same unit of work for now, note the caveat).
- [ ] Add a trivial subscriber that logs received events to prove the round-trip in the dashboard.
- [ ] Document the outbox limitation and file a follow-up for a transactional outbox (Milestone 8).

## Milestone 4 — Conference Management service (CRUD warm-up)

Goal: a second service, EF Core CRUD, publishing integration events the others consume.

- [ ] Scaffold `src/Services/ConferenceManagement/ConferenceManagement.{Api,Domain,Tests}`.
- [ ] EF Core model: `Conference`, `SeatType`; migrations; register Postgres db in AppHost.
- [ ] CRUD endpoints (create/publish conference, add seat types) with validation + typed results.
- [ ] Publish `ConferenceCreated`, `ConferencePublished`, `SeatCreated` integration events.
- [ ] Have Registration subscribe to `SeatCreated` to seed `AddSeats` (contexts now talk).

## Milestone 5 — Orders & Registration process (the CQRS showpiece)

Goal: the saga that coordinates ordering, seat reservation, and payment.

- [ ] Scaffold `src/Services/Ordering/Ordering.{Api,Domain,Tests}` (event-sourced, Marten).
- [ ] `Order` aggregate: place order, mark reserved, confirm, expire (with domain events).
- [ ] `RegistrationProcessManager` (saga): OrderPlaced → reserve seats → await payment → confirm/cancel, with a reservation timeout.
- [ ] Endpoints to place an order and query its status.
- [ ] Integration events across Ordering ↔ Registration ↔ Payments.
- [ ] Saga tests covering the happy path, timeout, and rejection.

## Milestone 6 — Payments service

- [ ] Scaffold `src/Services/Payments/Payments.{Api,Domain,Tests}` (EF Core CRUD).
- [ ] Simulate a payment flow (initiate → complete/reject) emitting integration events.
- [ ] Wire into the Ordering saga.

## Milestone 7 — Read side + frontend

Goal: something a human can click.

- [ ] Marten async projections for read models (priced order, conference view, seat availability).
- [ ] A read API (or read endpoints on each service) serving the projections.
- [ ] A Blazor (or minimal SPA) web frontend registered in the AppHost; optionally YARP as a gateway.
- [ ] End-to-end: browse a published conference → order seats → see reservation → pay → confirmed.

## Milestone 8 — Hardening & cross-cutting

Pull from these as needed; not strictly sequential.

- [ ] Transactional outbox (replace the outbox-lite from Milestone 3).
- [ ] Authentication/authorization (Identity or a Keycloak/OIDC Aspire resource).
- [ ] Resilience + retry policies on integration handlers; idempotent consumers.
- [ ] Broaden `Aspire.Hosting.Testing` integration tests across services.
- [ ] Deployment: `azd` / Aspire manifest to Azure Container Apps (or Aspir8 for k8s).
- [ ] Structured docs under `docs/` (per-context READMEs, event catalog).

---

## Suggested cadence for low-activity stretches

Each milestone breaks into ~1-hour tasks. If you only have one short session, the best single wins in order are: **M0 EventBus.csproj
fix** → **M1 AppHost + dashboard** (highest morale payoff) → **M2 first persisted aggregate**. After that the pattern repeats per
service, so momentum compounds.
