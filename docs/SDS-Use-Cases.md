# Somtum Zabb Delivery — SDS Use Cases

**Document status:** Draft 0.1 — for review
**Scope:** Use-case analysis only. This document does not define the API, database schema, or detailed architecture.

Architecture, component/deployment diagrams, and runtime sequences are documented in [SDS-Architecture.md](./SDS-Architecture.md).

## 1. Purpose and scope

This draft turns the supplied Game Design Document (GDD) and follow-up game/API proposals into use cases for Somtum Zabb Delivery. It covers the game client and its interactions with the backend, plus the existing Back Office workflows. It does not specify the GDevelop Event Sheet implementation.

Requirements are labelled to distinguish their source and maturity:

- **Existing backend:** behavior already implemented in the Somtumzing backend project.
- **GDD:** behavior stated in the supplied game design document.
- **Added proposal:** behavior from the follow-up scoring, patience, Chaos Engine, or online/PVP descriptions.
- **Open:** conflict, missing rule, or decision needed before implementation.

An item being described here does not mean it has already been implemented or finally approved.

## 2. Actors and system boundary

| Actor | Role |
|---|---|
| Player | Plays a game day, manages orders, responds to events, and may enter competitive modes. |
| Game Client | Presents game screens and sends player actions to backend services where persistence, competition, or validation is required. |
| Backend | Authenticates players, manages authoritative persisted state, validates eligible game results/actions, and records important events. |
| Back Office Administrator | Searches player accounts, changes player account status, and reviews audit events. |
| Matchmaking/Competition Service | Proposed backend responsibility for creating matches, assigning competitors, and coordinating competitive results. |
| Platform Store / Rewarded Ad Provider | External actors mentioned by the F2P proposal; integration details are not specified. |

The game client owns presentation and moment-to-moment input. Persisted currency, progression, submitted competitive results, and other valuable state must not be accepted solely on the basis of client-calculated values. The exact server authority model is an open design item.

## 3. Use-case catalogue

| ID | Use case | Primary actor | Source / status |
|---|---|---|---|
| UC-01 | Create or resume a player session | Player | Existing backend: guest creation, token refresh |
| UC-02 | Start a sales day and load stage rules | Player | GDD and added API proposal |
| UC-03 | Receive and manage customer orders | Player | GDD |
| UC-04 | Prepare, serve, and score an order | Player | GDD and added scoring proposal |
| UC-05 | Handle customer patience and a Chaos event | Player | GDD and added patience/Chaos proposals |
| UC-06 | Settle a sales day and save progress | Player | GDD and added economy proposal |
| UC-07 | Unlock a stage and buy an upgrade | Player | GDD |
| UC-08 | Submit a weekly challenge result and view rankings | Player | Added online proposal |
| UC-09 | Play a real-time competitive match | Player | Added online proposal |
| UC-10 | Send or counter a PVP sabotage card | Player | Added Sabotage proposal |
| UC-11 | Find, suspend, or reactivate a player | Back Office Administrator | Existing backend |
| UC-12 | Search audit events | Back Office Administrator | Existing backend |

UC-08 through UC-10 are proposals, not current backend capabilities. No real-money purchase flow is specified in this draft.

## 4. Detailed use cases

### UC-01 — Create or resume a player session

**Primary actor:** Player
**Supporting actor:** Game Client
**Source / status:** Guest creation and refresh are implemented in the backend. Provider account linking is not implemented.

**Preconditions**

- The player has network access to the service.
- To resume, the client holds a valid refresh token.

**Main flow**

1. A new player opens the game and requests a guest account.
2. The backend creates the player identity and issues a short-lived access token and refresh token.
3. The client keeps the credentials according to the platform's secure-storage capabilities.
4. When the access token expires, the client submits its refresh token.
5. The backend rotates the refresh token and returns a new access token and refresh token.

**Alternative / failure flows**

- A suspended player cannot refresh a session; the backend records the rejected attempt.
- An invalid, expired, or already-revoked refresh token is rejected.
- A guest who loses their credentials cannot recover the account using a linked identity provider yet.

**Postconditions**

- The player has an authenticated session, or receives an explicit authentication failure.
- Credentials and secrets are not written to audit event details.

### UC-02 — Start a sales day and load stage rules

**Primary actor:** Player
**Supporting actors:** Game Client, Backend
**Source / status:** Stage progression and day play are in the GDD; a stage-config endpoint is proposed.

**Preconditions**

- The player has an active account.
- The selected stage is unlocked.

**Main flow**

1. The player selects an available stage from the progression map.
2. The client requests the stage's rules and presentation configuration.
3. The backend returns the stage identity, unlock requirement, duration, queue/table limits, target, and enabled mechanics.
4. The client presents the stage and starts the day.
5. The game session is associated with the player, stage, and a unique day/session identity.

**Alternative / failure flows**

- A locked or unknown stage is rejected.
- A configuration version mismatch is reported so the client does not silently play with incompatible rules.
- If the player loses connectivity, whether a day may continue offline and later synchronize is **Open**.

**Postconditions**

- A day is active with a known stage configuration and a unique session identity.
- The source of truth for session start time, random seed, and stage configuration version is **Open**.

### UC-03 — Receive and manage customer orders

**Primary actor:** Player
**Supporting actor:** Game Client
**Source / status:** The GDD defines walk-in and delivery orders, with a queue limit of three.

**Preconditions**

- A sales day is active.

**Main flow**

1. Customers or delivery requests arrive according to the stage's rules.
2. The client displays each order and its requested menu and taste requirements.
3. The player selects an order and prepares it before its patience expires.
4. The client updates the visible queue and customer mood as time passes.

**Alternative / failure flows**

- If the queue is full, the behavior for a fourth incoming order must follow the approved rule. A follow-up proposal says it disappears and costs one review star; confirm before treating this as final.
- A customer may leave at zero patience; the exact relationship between a walkout, a dropped order, and star loss is **Open**.

**Postconditions**

- Each order is either active, served, cancelled, or otherwise resolved exactly once.
- The authoritative event identity and duplicate-order handling are **Open**.

### UC-04 — Prepare, serve, and score an order

**Primary actor:** Player
**Supporting actors:** Game Client, Backend
**Source / status:** Core loop is in the GDD; per-order price and scoring formulas are an added proposal.

**Preconditions**

- A sales day and an unserved order are active.

**Main flow**

1. The player prepares the selected menu and taste profile.
2. The player serves the order or hands it off for delivery.
3. The system compares the prepared result with the requested result.
4. The system calculates order revenue, customer satisfaction, and any earned stars.
5. The client presents the result and advances the order to a terminal served state.

**Added proposal rules**

- Base menu prices are specified for Som Tam Zing (฿50), larb/nam tok (฿60), grilled chicken (฿80), and selected sides (฿10).
- Exact taste match adds ฿20; each deviation unit deducts ฿10.
- Cash/ordinary QR uses multiplier 1.0; Pao Tung uses 1.20; delivery uses 1.15.
- The supplied example calculates (฿50 + ฿20) × 1.20 = ฿84.

**Alternative / failure flows**

- A critical spice mismatch may set revenue to zero and reduce satisfaction.
- A payment-provider outage may delay or fail payment; whether the order is served while settlement is pending is **Open**.
- The backend must not trust a client-provided final price, rating, or reward without a validation rule.

**Postconditions**

- The order has one result and one corresponding revenue outcome.
- Whether payment and delivery multipliers stack, and how non-spice ingredient deviations are scored, are **Open**.

### UC-05 — Handle customer patience and a Chaos event

**Primary actor:** Player
**Supporting actors:** Game Client, Backend
**Source / status:** Patience and Chaos rules are specified in follow-up proposals; authoritative timing is not finalized.

**Preconditions**

- A sales day is active and a customer or eligible event condition exists.

**Main flow**

1. A customer's patience begins at 100% and decays over time.
2. The client shows the customer's state: HAPPY (70–100%), IMPATIENT (30–69%), ANGRY (1–29%), or zero-patience behavior.
3. Active conditions such as smoke, weather, customer type, or payment delay may modify the decay rate.
4. The player responds to a triggered event using its eligible action before its response window ends.
5. The system resolves the event as successful, failed, or still active, and applies its effects once.
6. The client presents the result and updates relevant customers, orders, and day state.

**Alternative / failure flows**

- If the response window expires, the event's configured penalty is applied once.
- If an action is submitted late, duplicated, or for the wrong event, the system rejects or returns the existing result; precise behavior is **Open**.
- At zero patience, a customer may leave, reduce rating, or trigger a chaos effect. The exact combined outcome and rating calculation are **Open**.
- Multiple simultaneous events may stack, queue, or be capped; the synergy formula is **Open**.

**Candidate Chaos events from the supplied registry**

| Event | Trigger / response | Candidate effect |
|---|---|---|
| Municipal inspector | Eligible from stage 2; respond with `FLEE_CART` within 3 seconds | Fine and warning ticket if missed |
| Pao Tung glitch | Pao Tung payment; 25% proposed outage chance; respond within 5 seconds | Payment delay or order consequence, still to specify |
| Charcoal smoke | Grill active for more than 15 seconds; clear with exhaust fan | Obscured kitchen and 1.35 patience decay multiplier |
| Karaoke rage | Customer patience below 30%; respond within 8 seconds | Karaoke damage/noise and review consequence |
| Rain flood | Random weather in stages 3–4; proposed 30 seconds | Delivery speed reduced by 50%; fewer walk-ins |
| Stray dog attack | Delivery passes the dog's location; proposed 4 seconds | Delivery delay or one spilled order; counter with grilled chicken |

The registry says eight events but currently lists six. Other proposed events, including blackout/button shuffle, require reconciliation with the registry. Bribery is excluded from the accepted action set pending resolution because the original GDD says that bribery is not an in-game solution.

**Postconditions**

- Event state transitions once from scheduled/idle to warning, then resolved or active penalty.
- Any financial, warning, patience, or progression effect is applied once and can be traced to the event instance.

### UC-06 — Settle a sales day and save progress

**Primary actor:** Player
**Supporting actors:** Game Client, Backend
**Source / status:** End-of-day and net-profit concepts are in supplied material; settlement details are incomplete.

**Preconditions**

- A sales day has ended or has otherwise reached a terminal state.

**Main flow**

1. The system closes eligible orders and pending events.
2. The system totals revenue and applies costs, fines, repair costs, and eligible delayed payments.
3. The system calculates the daily review result and updates persistent player progression.
4. The client displays a receipt-style day summary and reviews.

**Alternative / failure flows**

- Repeated settlement for the same session must return the prior result rather than grant currency or stars again.
- An incomplete or disconnected session may be resumed, settled, or invalidated under a policy that is **Open**.

**Open rules**

- The added proposal says a day ends at 22:00, but the GDD also describes short 3–5 minute rounds; establish whether 22:00 is in-game time or a day-end label.
- Raw material cost is proposed as 35% of base-price sales; the treatment of multipliers, refunds, and failed orders is not specified.
- Stage completion refers to both target sales and cumulative stars, but the exact pass condition is not specified.
- Daily rating formula, earned-star conversion, carry-over rating, and zero-star game-over timing are not fully defined.

### UC-07 — Unlock a stage and buy an upgrade

**Primary actor:** Player
**Supporting actors:** Game Client, Backend
**Source / status:** Stage thresholds and upgrade/economy concepts are in the GDD; persistence and purchase rules are open.

**Preconditions**

- The player has a persistent profile and has completed the relevant prior play.

**Main flow**

1. The player opens the progression map or upgrade shop.
2. The game shows available stages, requirements, item costs, and effects.
3. The player selects an unlocked stage or an affordable upgrade.
4. The system validates eligibility and balance.
5. The system records the purchase/unlock once and updates the player's profile.

**Alternative / failure flows**

- A locked stage, insufficient balance, or duplicate purchase is rejected without changing player state.
- If a paid currency or store purchase is involved, receipt validation and refund handling require a separate use case.

**Open rules**

- Proposed unlock thresholds are 15, 40, and 75 stars.
- The GDD describes baht, gold chili, and neighborhood goodwill; balances, earning/spending rules, upgrade prices, and cross-device behavior need definition.
- Confirm which upgrades affect casual play and which are normalized or cosmetic-only in competitive modes.

### UC-08 — Submit a weekly challenge result and view rankings

**Primary actor:** Player
**Supporting actors:** Game Client, Backend / Competition Service
**Source / status:** Added asynchronous leaderboard proposal; not implemented.

**Preconditions**

- A challenge is active and the player has an eligible account.

**Main flow**

1. The player downloads the challenge stage, rules, and challenge identity.
2. The player completes one eligible run.
3. The client submits the run identity and result evidence.
4. The service validates eligibility and records no more than one accepted result per allowed attempt.
5. The player views the weekly ranking and eligible rewards.

**Alternative / failure flows**

- Expired challenges, duplicate submissions, invalid results, or unavailable ranking service return an explicit outcome.
- The result cannot be accepted solely because the client reports a high score.

**Open rules**

- Define attempt limits, authoritative score validation, ranking tie-breaks, season boundaries, reward delivery, and retention.
- Choose one source of truth for rankings; the GDevelop built-in leaderboard and a separate backend/Firebase leaderboard should not independently own canonical scores.

### UC-09 — Play a real-time competitive match

**Primary actor:** Player
**Supporting actors:** Game Client, Matchmaking/Competition Service, opponent
**Source / status:** Added 1v1/2v2 PVP proposal; not implemented.

**Preconditions**

- Players are authenticated and eligible for the selected mode.
- Match rules, normalized competitive stats, and a shared challenge/seed are available.

**Main flow**

1. A player queues for a selected mode.
2. The matchmaking service groups eligible players by mode and rating.
3. The service creates a match, assigns participants, and provides the shared order/event seed and ruleset version.
4. Participants play the match and exchange state/actions through the selected online transport.
5. The service validates the match result and determines winner, loss, draw, or invalid match.
6. The service updates rating/rank and stores the result.

**Alternative / failure flows**

- A participant disconnects, times out, or leaves; forfeit/reconnect/rematch behavior is **Open**.
- If participants report inconsistent outcomes, the server-side authoritative result wins or the match is invalidated according to a policy still to define.

**Open rules**

- Select supported initial mode (1v1, 2v2, or both), networking/authority model, match duration, rematch rules, MMR formula, rank season, and abuse handling.
- Shared random seed provides comparable inputs but does not by itself prove that clients played or reported results honestly.

### UC-10 — Send or counter a PVP sabotage card

**Primary actor:** Player
**Supporting actors:** Game Client, Matchmaking/Competition Service, opponent
**Source / status:** Added Sabotage proposal; not implemented.

**Preconditions**

- A competitive match is active.
- The player has an eligible deck, sufficient Chaos Meter, and no applicable restriction such as an immunity window.

**Main flow**

1. During pre-match setup, each player selects three attack cards and one defense card.
2. Eligible gameplay actions add Chaos Meter according to the match rules.
3. The player selects an attack card when its energy cost is available.
4. The service validates the card, meter, target, timing, cooldown/immunity, and match state.
5. The opponent receives the event and responds using an allowed counter within its response window.
6. The service resolves the effect, consumes the appropriate resource, and applies any immunity window.

**Alternative / failure flows**

- An invalid card, insufficient meter, invalid target, late action, duplicate command, or protected target is rejected without applying a second effect.
- A defense card may block an eligible attack automatically; exact use count and behavior are **Open**.
- Simultaneous attacks, effect stacking, disconnect during response, and 2v2 team targeting are **Open**.

**Added proposal values**

- Meter gains: exact match +15%, green-state delivery +10%, three-order speed combo +25%.
- Deck: three attack cards and one defense card.
- Card tiers: proposed costs of 25–35%, 50–65%, and 100%.
- Successful counter: proposed four-second immunity window.

These values need a single approved ruleset, including meter cap, starting value, stacking, deck reuse, card cooldowns, and precise counter-success criteria. Sabotage is a simulated in-game effect; it never installs or sends software to another player's device.

### UC-11 — Find, suspend, or reactivate a player

**Primary actor:** Back Office Administrator
**Source / status:** Player search and suspension/reactivation are implemented in the backend.

**Preconditions**

- Administrator is authenticated and authorized.
- Target account is a player account.

**Main flow**

1. Administrator searches by username, email, or player ID and filters active/suspended status.
2. Administrator reviews the account and current status.
3. Administrator selects suspend or reactivate and enters a reason.
4. The system validates the target and records the state change and `AdminAction` audit entry.
5. While suspended, the player's access token is rejected and refresh is denied.
6. On reactivation, a refresh token can be used again; access tokens issued before suspension remain invalid.

**Alternative / failure flows**

- Missing target, insufficient reason, repeated same-state action, or invalid administrator session does not change account state.

**Postconditions**

- Account status, administrator, timestamp, and reason are available to authorized Back Office workflows/audit.

### UC-12 — Search audit events

**Primary actor:** Back Office Administrator
**Source / status:** Audit Log search page is implemented.

**Preconditions**

- Administrator is authenticated and authorized.

**Main flow**

1. Administrator opens Audit Logs.
2. Administrator filters by event type, severity, UTC date range, IP address, or username.
3. The system displays matching events in pages of 50, newest first.
4. Administrator reviews outcome, target, request ID, and safe event details.

**Alternative / failure flows**

- Invalid date range is rejected with validation feedback.
- Data older than configured retention (180 days by default) is removed by the retention job.

## 5. Cross-cutting use-case rules

1. **Identity:** every player-owned operation is associated with the authenticated player; a client-supplied player ID is not proof of identity.
2. **Idempotency:** order resolution, event resolution, daily settlement, purchases, challenge submissions, and match results must define duplicate-request behavior before implementation.
3. **Auditability:** administrator actions and important security/game outcomes should be traceable without storing credentials, access tokens, refresh tokens, or unnecessary personal data.
4. **Competitive fairness:** challenge rules and match seeds are versioned/shared, while valuable outcomes are validated by a trusted service.
5. **Separation of modes:** casual progression, competitive normalized stats, and cosmetic purchases must have explicit rules about which state is shared.
6. **No bribery rule:** the source GDD says bribery is not an in-game solution. Use `FLEE_CART` or a formally approved lawful fine rule; do not include a bribe action unless the GDD is explicitly revised.

## 6. Decisions to resolve before detailed design

1. Which document/version is authoritative when the GDD and follow-up proposals conflict?
2. Is the initial release single-player only, or does it include weekly leaderboard and/or real-time PVP?
3. For the inspector event, is the only response fleeing, or may the player pay an official fine? Bribery remains excluded by the original GDD.
4. What exact calculation maps order results and customer satisfaction into earned stars, daily rating, stage unlocks, and game over?
5. How do payment bonuses stack with delivery, and which item/taste differences affect price?
6. What is the authoritative online model for time, random events, event responses, scoring, settlement, and match outcomes?
7. What are the final Chaos registry, synergy formula/cap, event overlap rules, and timeout/retry semantics?
8. What are the challenge attempt/reward rules and PVP matchmaking, rank, disconnect, and anti-cheat rules?
9. Which progression and upgrade effects are persistent, and which are normalized in competitive modes?
