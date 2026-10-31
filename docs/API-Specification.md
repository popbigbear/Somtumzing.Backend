# Somtum Zabb Delivery — API Specification

**Document status:** Draft 0.2 — detailed API proposal for review
**Scope:** REST endpoint contracts, authorization, request/response examples, and proposed processing behavior.
**Important:** Only endpoints labelled **Implemented** exist in the current backend. All other endpoints and gameplay rules are proposals, not deployed behavior or final game decisions.

## 1. Contract conventions

### 1.1 Common conventions

- Base path: `/api/v1`; health paths are exceptions.
- Request and response property names use `camelCase`.
- JSON requests use `Content-Type: application/json`.
- Timestamps are ISO 8601 UTC values, for example `2026-10-04T12:00:00Z`.
- IDs shown as UUIDs are strings. Human-readable codes such as `STAGE_02_MARKET` are stable catalog codes.
- Unless an endpoint says otherwise, a successful resource response is a JSON object, not wrapped in a `{ "status", "data" }` envelope.
- Query pagination, where offered, uses `limit` and opaque `cursor`; server defaults and maximums are stated per endpoint.
- Money is represented as decimal JSON numbers in baht. Currency balances must not be calculated by trusting client-submitted totals.
- Unknown fields may be ignored for forward compatibility; required fields, enum values, and maximum sizes are validated.

### 1.2 Authorization classes

| Name | Meaning |
|---|---|
| Public | No player credential required. Public endpoints must not expose private player data. |
| Player | `Authorization: Bearer <accessToken>` for an active player account. The caller's identity comes from the validated token, not a submitted player ID. |
| Internal | Service-to-service credential and a narrow scope. Must not be callable with a normal player token. |
| Back Office Admin | The current Back Office uses its authenticated Admin cookie on Razor Pages. No Back Office REST routes are defined in this draft. |

The current player JWT is short-lived. Suspended users and tokens with a stale security stamp are rejected. Refresh credentials are sent only in the refresh request body and must never be logged.

### 1.3 Common errors

Proposed endpoints use `application/problem+json` for errors:

```json
{
  "type": "about:blank",
  "title": "Request could not be processed.",
  "status": 409,
  "detail": "The game session is already settled.",
  "instance": "/api/v1/game-sessions/8cfbc870-344e-46aa-8b38-cf0dfb3b2d2a/settle",
  "code": "SESSION_ALREADY_SETTLED",
  "traceId": "00-..."
}
```

Common meanings: `400` invalid request, `401` missing/invalid player credential, `403` authenticated but not allowed (including suspended account), `404` resource absent or not visible to caller, `409` conflicting state/duplicate operation, `429` rate limit, and `500` unexpected server failure. Validation errors include field-level errors. Error wording is not a stable machine contract; `code` is.

### 1.4 Idempotency and concurrency (proposed)

- Mutating operations that create sessions, resolve orders/events, settle days, purchase upgrades, submit challenge results, queue for matches, send sabotage, or counter sabotage require an `Idempotency-Key` UUID header.
- Repeating the same key and identical request returns the original result. Reusing the key with a different request returns `409 IDEMPOTENCY_KEY_REUSED`.
- State-changing actions include a resource version or sequence number where needed. A stale sequence returns `409 STALE_ACTION`.
- A match or session action is accepted at most once. A retry must not award currency, stars, warnings, or effects more than once.

These conventions are proposed for the new APIs; they are not all implemented by the current backend.

## 2. Implemented endpoints

### 2.1 `GET /health/live`

**Meaning:** Liveness probe; confirms the process is running.
**Authorization:** Public.
**Parameters/request:** None.
**Success:** `200 text/plain`, body `Healthy`.
**Failure:** Normally none while the process is alive.

### 2.2 `GET /health/ready`

**Meaning:** Readiness probe; confirms required dependencies, currently PostgreSQL, are available.
**Authorization:** Public.
**Parameters/request:** None.
**Success:** `200 text/plain`, body `Healthy`.
**Failure:** `503` when a readiness dependency fails.

### 2.3 `GET /api/v1/system/status`

**Meaning:** Basic API service status.
**Authorization:** Public.
**Parameters/request:** None.
**Success `200`:**

```json
{ "status": "ok", "service": "somtumzing-backend-api" }
```

**Failure:** Standard unexpected-error response.

### 2.4 `POST /api/v1/auth/guest`

**Meaning:** Creates a new guest account and starts a player session.
**Authorization:** Public; rate-limited by client IP (currently 20 requests/minute).
**Parameters/request:** No body.
**Success `200`:**

```json
{
  "playerId": "38b9fd0d-474a-44ea-9f48-c2f5bf0db322",
  "accessToken": "<short-lived-token>",
  "refreshToken": "<opaque-secret>",
  "accessTokenExpiresAtUtc": "2026-10-04T12:15:00Z",
  "refreshTokenExpiresAtUtc": "2026-11-03T12:00:00Z"
}
```

**Failure:** `429` rate limit; `500` if account/session creation fails. The endpoint does not link an existing account or recover a guest account.

### 2.5 `POST /api/v1/auth/refresh`

**Meaning:** Rotates a refresh credential and issues a new access/refresh pair.
**Authorization:** Public endpoint; possession of the refresh credential is required.
**Request:**

```json
{ "refreshToken": "<opaque-secret>" }
```

`refreshToken` is required, 64–128 characters.

**Success `200`:** Same response shape as `POST /api/v1/auth/guest`.
**Failure:** `400` invalid request shape; `401` invalid, expired, revoked, or suspended account; `500` unexpected failure.

### 2.6 `POST /api/v1/auth/logout`

**Meaning:** Revokes the submitted refresh credential belonging to the authenticated player.
**Authorization:** Player.
**Request:**

```json
{ "refreshToken": "<opaque-secret>" }
```

**Success:** `204 No Content`. Repeating logout for an already revoked/unknown token is also `204`.
**Failure:** `400` invalid request; `401` invalid access token.

### 2.7 `GET /api/v1/auth/me`

**Meaning:** Returns the authenticated player's basic profile.
**Authorization:** Player.
**Parameters/request:** None.
**Success `200`:**

```json
{
  "playerId": "38b9fd0d-474a-44ea-9f48-c2f5bf0db322",
  "createdAtUtc": "2026-10-04T12:00:00Z"
}
```

**Failure:** `401` invalid, expired, revoked-by-suspension, or otherwise invalid token.

## 3. Proposed player progression and stage endpoints

### 3.1 `GET /api/v1/player/progression`

**Meaning:** Reads saved stage unlocks, cumulative stars, and current progression for the caller.
**Authorization:** Player.
**Query:** None.
**Success `200` (proposed):**

```json
{
  "playerId": "38b9fd0d-474a-44ea-9f48-c2f5bf0db322",
  "unlockedStageIds": ["STAGE_01_HOME_ALLEY"],
  "currentStageId": "STAGE_01_HOME_ALLEY",
  "cumulativeStars": 0,
  "updatedAtUtc": "2026-10-04T12:00:00Z"
}
```

**Processing:** Read only; player identity is obtained from the token. Returns no other player's progression.

### 3.2 `GET /api/v1/stages`

**Meaning:** Lists stage catalog entries and their unlock thresholds.
**Authorization:** Public.
**Query:** Optional `locale` (BCP 47 language tag; default `th-TH`).
**Success `200` (proposed):**

```json
{
  "stages": [
    {
      "stageId": "STAGE_01_HOME_ALLEY",
      "name": "ซอยบ้านป้าส้ม",
      "unlockStars": 0,
      "durationSeconds": 180
    },
    {
      "stageId": "STAGE_02_MARKET",
      "name": "ตลาดนัดหลังเลิกงาน",
      "unlockStars": 15,
      "durationSeconds": 240
    }
  ]
}
```

**Processing:** Returns published stages only. Localization fallback behavior is to `th-TH`.

### 3.3 `GET /api/v1/stages/{stage_id}/config`

**Meaning:** Reads a stage's published gameplay configuration.
**Authorization:** Public.
**Path parameter:** `stage_id` — stage catalog code.
**Query:** Optional `configVersion`; omitted returns current published version.
**Success `200` (proposed):**

```json
{
  "stageId": "STAGE_04_TEMPLE_FAIR",
  "stageName": "งานวัดประจำปี",
  "configVersion": 1,
  "requiredUnlockStars": 75,
  "targetSalesAmount": 12000.00,
  "dayDurationSeconds": 300,
  "maxTableCapacity": 6,
  "deliveryQueueLimit": 3,
  "rawMaterialCostPct": 0.35,
  "activeChaosMechanics": [
    "PAO_TUNG_APP_OUTAGE",
    "BLACKOUT_BUTTON_SHUFFLE",
    "LOCAL_DOG_PURSUIT"
  ]
}
```

**Processing:** Returns immutable versioned configuration. Unknown stage/version returns `404`. Whether hidden/unlocked stage configs are public is an open content-protection decision; this draft treats published config as public.

## 4. Proposed sales-day and order endpoints

### 4.1 `POST /api/v1/game-sessions`

**Meaning:** Starts a casual sales-day session for an unlocked stage.
**Authorization:** Player.
**Headers:** `Idempotency-Key` required.
**Request:**

```json
{
  "stageId": "STAGE_01_HOME_ALLEY",
  "clientBuild": "1.0.0"
}
```

`stageId` is required. `clientBuild` is optional diagnostic metadata.

**Success `201` (proposed):**

```json
{
  "sessionId": "8cfbc870-344e-46aa-8b38-cf0dfb3b2d2a",
  "stageId": "STAGE_01_HOME_ALLEY",
  "status": "ACTIVE",
  "configVersion": 1,
  "startedAtUtc": "2026-10-04T12:00:00Z",
  "endsAtUtc": "2026-10-04T12:03:00Z",
  "rulesetId": "casual-v1"
}
```

**Processing:** Server creates one session using a published stage config and captures its version. The returned wall-clock times are server-generated. Whether gameplay may continue offline is not decided here.

**Failures:** `401`; `403` stage locked; `404` unknown stage; `409` another active session is not allowed; `429` session-start limit.

### 4.2 `GET /api/v1/game-sessions/{session_id}`

**Meaning:** Reads the caller's session state and summary.
**Authorization:** Player; session owner only.
**Path:** `session_id` — UUID.
**Success `200` (proposed):**

```json
{
  "sessionId": "8cfbc870-344e-46aa-8b38-cf0dfb3b2d2a",
  "stageId": "STAGE_01_HOME_ALLEY",
  "status": "ACTIVE",
  "startedAtUtc": "2026-10-04T12:00:00Z",
  "endsAtUtc": "2026-10-04T12:03:00Z",
  "servedOrderCount": 0,
  "grossRevenue": 0.00,
  "activeChaosEventCount": 0,
  "version": 1
}
```

**Processing:** Read only. `version` increases when authoritative session state changes. Unknown or non-owned session returns `404`.

### 4.3 `POST /api/v1/orders/calculate-score`

**Meaning:** Resolves one order in an active session and returns its server-calculated score and revenue. Despite the historical name, this draft treats this as a state-changing order resolution, not a client-side preview.
**Authorization:** Player; must own the session.
**Headers:** `Idempotency-Key` required.
**Request:**

```json
{
  "sessionId": "8cfbc870-344e-46aa-8b38-cf0dfb3b2d2a",
  "orderId": "b6f92811-836e-4260-8db5-006e0d5a2d6e",
  "menuCode": "SOMTAM_ZING",
  "requestedTaste": {
    "spiceLevel": 5
  },
  "preparedTaste": {
    "spiceLevel": 5
  },
  "customerPatienceRemainingPct": 82.5,
  "paymentMethod": "PAO_TUNG_APP",
  "isDelivery": false
}
```

The order must have been issued for this session. Client-reported patience is provisional input and cannot by itself determine a ranked or monetary outcome.

**Success `200` (proposed):**

```json
{
  "sessionId": "8cfbc870-344e-46aa-8b38-cf0dfb3b2d2a",
  "orderId": "b6f92811-836e-4260-8db5-006e0d5a2d6e",
  "result": "SERVED",
  "basePrice": 50.00,
  "accuracyBonus": 20.00,
  "deviationPenalty": 0.00,
  "paymentMultiplier": 1.20,
  "finalOrderRevenue": 84.00,
  "customerSatisfactionScore": 5.0,
  "patienceState": "HAPPY",
  "earnedStars": 3,
  "sessionVersion": 2
}
```

**Processing:** Backend loads the server-issued order/menu/ruleset, validates that it is unresolved, calculates the result, and records one result. Repeated same-key request returns the prior result. The formulas and conversion of satisfaction to stars remain provisional pending game-rule approval.

**Failures:** `400` malformed/unsupported values; `401`; `403` wrong session owner; `404` session/order absent; `409` inactive session, already resolved order, or stale session version.

### 4.4 `POST /api/v1/game-sessions/{session_id}/settle`

**Meaning:** Closes a session, computes its final result, and persists eligible progression/rewards.
**Authorization:** Player; session owner only.
**Path:** `session_id` — UUID.
**Headers:** `Idempotency-Key` required.
**Request:**

```json
{
  "clientFinalState": {
    "unresolvedOrderIds": []
  },
  "expectedVersion": 8
}
```

The request must not supply authoritative net profit, wallet balance, stars, or rank. `clientFinalState` is advisory only; `expectedVersion` detects stale state.

**Success `200` (proposed):**

```json
{
  "sessionId": "8cfbc870-344e-46aa-8b38-cf0dfb3b2d2a",
  "status": "SETTLED",
  "grossRevenue": 250.00,
  "rawMaterialCost": 87.50,
  "fines": 40.00,
  "repairCosts": 0.00,
  "netProfit": 122.50,
  "dailyRating": 4.2,
  "earnedStars": 3,
  "walletBalances": {
    "THB": 122.50,
    "GOLD_CHILI": 0
  },
  "settledAtUtc": "2026-10-04T12:03:00Z"
}
```

**Processing:** Server resolves pending event/order outcomes according to the selected ruleset, calculates settlement, commits session closure and rewards atomically, and returns the existing result on a retry. This draft proposes `409 SESSION_NOT_FINISHED` if settlement is requested before the session end; early-abandon policy remains to be approved.

## 5. Proposed Chaos Engine endpoints

### 5.1 `POST /api/v1/chaos/eval-trigger`

**Meaning:** Asks the backend to evaluate the next eligible Chaos trigger for an active session.
**Authorization:** Player; session owner only.
**Headers:** `Idempotency-Key` required per evaluation checkpoint.
**Request:**

```json
{ "sessionId": "8cfbc870-344e-46aa-8b38-cf0dfb3b2d2a" }
```

The client does not submit elapsed time, active event count, or authoritative kitchen/payment state; the backend derives these from its session/action records.

**Success `200` (proposed, no event):**

```json
{
  "sessionId": "8cfbc870-344e-46aa-8b38-cf0dfb3b2d2a",
  "eventTriggered": false,
  "nextEvaluationAtUtc": "2026-10-04T12:00:22Z"
}
```

**Success `200` (proposed, event):**

```json
{
  "sessionId": "8cfbc870-344e-46aa-8b38-cf0dfb3b2d2a",
  "eventTriggered": true,
  "event": {
    "eventInstanceId": "bdb968e2-ed79-457a-9bb3-d20846b09d72",
    "eventCode": "MUNICIPAL_INSPECTOR",
    "state": "WARNING",
    "triggeredAtUtc": "2026-10-04T12:00:20Z",
    "respondByUtc": "2026-10-04T12:00:23Z",
    "requiredActionCode": "FLEE_CART"
  }
}
```

**Processing:** Trigger eligibility, timing, randomness, and active-event limits are server-owned. This endpoint is intended for eligible checkpoints, not unrestricted high-frequency client polling. Exact scheduling and event registry are open rules.

### 5.2 `POST /api/v1/chaos/respond-action`

**Meaning:** Submits an action to resolve an active Chaos event.
**Authorization:** Player; session owner only.
**Headers:** `Idempotency-Key` required.
**Request:**

```json
{
  "sessionId": "8cfbc870-344e-46aa-8b38-cf0dfb3b2d2a",
  "eventInstanceId": "bdb968e2-ed79-457a-9bb3-d20846b09d72",
  "actionCode": "FLEE_CART"
}
```

Client-provided response duration is not authoritative; server receipt time is used.

**Success `200` (proposed):**

```json
{
  "eventInstanceId": "bdb968e2-ed79-457a-9bb3-d20846b09d72",
  "state": "RESOLVED",
  "result": "SUCCESS",
  "actionCode": "FLEE_CART",
  "resolvedAtUtc": "2026-10-04T12:00:21.800Z",
  "effects": {
    "financialImpactBaht": 0.00,
    "warningTicketsAdded": 0
  },
  "sessionVersion": 9
}
```

**Processing:** Action must be valid for the event and received before server deadline. Timeout processing is server-side and applies effects once. A late request returns `409 EVENT_ALREADY_RESOLVED` with the canonical resolution where possible. Inspector response in this draft is `FLEE_CART`; bribery is excluded in accordance with the original GDD.

### 5.3 `GET /api/v1/chaos/active-status/{session_id}`

**Meaning:** Reads active events and current server-calculated effect summaries for a session.
**Authorization:** Player; session owner only.
**Path:** `session_id` — UUID.
**Success `200` (proposed):**

```json
{
  "sessionId": "8cfbc870-344e-46aa-8b38-cf0dfb3b2d2a",
  "activeEvents": [
    {
      "eventInstanceId": "bdb968e2-ed79-457a-9bb3-d20846b09d72",
      "eventCode": "CHARCOAL_SMOKE",
      "state": "ACTIVE",
      "startedAtUtc": "2026-10-04T12:00:20Z",
      "endsAtUtc": "2026-10-04T12:00:26Z",
      "effects": { "patienceDecayMultiplier": 1.35 }
    }
  ],
  "synergyActive": false,
  "combinedEffects": { "patienceDecayMultiplier": 1.35 }
}
```

**Processing:** Read only; multipliers are calculated from server-owned active events and the published ruleset. Combined multiplier formula/cap remains open.

### 5.4 `POST /api/v1/chaos/force-trigger`

**Meaning:** Applies an explicitly selected event for an internal competitive/sabotage service or controlled test environment.
**Authorization:** Internal service credential with `chaos:force` scope. Not available to normal player tokens. Production debug access is disabled.
**Headers:** `Idempotency-Key` required.
**Request:**

```json
{
  "targetSessionId": "8cfbc870-344e-46aa-8b38-cf0dfb3b2d2a",
  "eventCode": "BLACKOUT_BUTTON_SHUFFLE",
  "source": {
    "type": "MATCH_SABOTAGE",
    "matchId": "efdb7626-9c4a-47b4-9b13-9bbfab7233d1",
    "participantId": "f2778f60-61b1-40b8-82ad-4ccf6918afef"
  }
}
```

**Success `202` (proposed):**

```json
{
  "targetSessionId": "8cfbc870-344e-46aa-8b38-cf0dfb3b2d2a",
  "eventInstanceId": "cb104f2d-55d8-42f6-ae75-8bafc4f2e09e",
  "eventCode": "BLACKOUT_BUTTON_SHUFFLE",
  "state": "WARNING",
  "durationSeconds": 5
}
```

**Processing:** The caller identity comes from service authentication; `source` is validated against the referenced match/participant and is audit-logged. Unknown or ineligible target returns `404`/`409`.

## 6. Proposed wallet and upgrade endpoints

### 6.1 `GET /api/v1/player/wallet`

**Meaning:** Reads the caller's current game-currency balances.
**Authorization:** Player.
**Success `200` (proposed):**

```json
{
  "balances": [
    { "currencyCode": "THB", "amount": 1200.00 },
    { "currencyCode": "GOLD_CHILI", "amount": 5 },
    { "currencyCode": "NEIGHBORHOOD_GOODWILL", "amount": 12 }
  ],
  "updatedAtUtc": "2026-10-04T12:00:00Z"
}
```

**Processing:** Read only. Balances are server-owned. Exact currencies, scale/precision, and earning/spending rules require game-rule approval.

### 6.2 `GET /api/v1/upgrades`

**Meaning:** Lists published upgrade catalog entries.
**Authorization:** Public.
**Query:** Optional `locale` (default `th-TH`) and `stageId`.
**Success `200` (proposed):**

```json
{
  "upgrades": [
    {
      "upgradeCode": "EXHAUST_FAN",
      "name": "พัดลมดูดควัน",
      "category": "EQUIPMENT",
      "currencyCode": "THB",
      "price": 500.00,
      "maxLevel": 3,
      "effectSummary": "Reduces smoke impact"
    }
  ]
}
```

**Processing:** Returns published items only. This endpoint does not disclose player-specific balances.

### 6.3 `POST /api/v1/player/upgrades/purchase`

**Meaning:** Purchases or upgrades one catalog item for the caller.
**Authorization:** Player.
**Headers:** `Idempotency-Key` required.
**Request:**

```json
{
  "upgradeCode": "EXHAUST_FAN",
  "quantity": 1
}
```

`quantity` defaults to 1 if omitted; proposed maximum is 1 per call.

**Success `201` (proposed):**

```json
{
  "purchaseId": "5cff77e4-d5e6-42d1-a03f-7efbd8d07d62",
  "upgradeCode": "EXHAUST_FAN",
  "newLevel": 1,
  "currencyCode": "THB",
  "amountDebited": 500.00,
  "remainingBalance": 700.00,
  "purchasedAtUtc": "2026-10-04T12:00:00Z"
}
```

**Processing:** Validate catalog, level/cap, and balance; debit and grant atomically; write a ledger entry and audit/business event. Cosmetic and gameplay upgrades must follow the approved mode rules. `409` insufficient funds, already at max, or item unavailable.

## 7. Proposed weekly challenge and leaderboard endpoints

### 7.1 `GET /api/v1/challenges/current`

**Meaning:** Retrieves the current published time-limited challenge.
**Authorization:** Public.
**Query:** Optional `mode` (default `WEEKLY`).
**Success `200` (proposed):**

```json
{
  "challengeId": "WEEK-2026-40",
  "mode": "WEEKLY",
  "stageId": "STAGE_04_TEMPLE_FAIR",
  "rulesetId": "challenge-v1",
  "startsAtUtc": "2026-10-04T00:00:00Z",
  "endsAtUtc": "2026-10-11T00:00:00Z",
  "status": "ACTIVE",
  "rewardSummary": "Cosmetic cart skin"
}
```

**Processing:** Returns current challenge metadata only. If no challenge is active, return `204 No Content`. Challenge time zone/week boundary must be configured consistently.

### 7.2 `POST /api/v1/challenges/{challenge_id}/runs`

**Meaning:** Registers a new run attempt for the specified challenge and issues a server-controlled run identity. This changes the earlier inventory's “submit completed run” meaning to include a start step so the backend can bind later results to an eligible attempt.
**Authorization:** Player.
**Path:** `challenge_id` — challenge code/UUID.
**Headers:** `Idempotency-Key` required.
**Request:**

```json
{
  "clientBuild": "1.0.0"
}
```

**Success `201` (proposed):**

```json
{
  "runId": "51137b45-3b42-4d7f-a2c5-41193e6cecd5",
  "challengeId": "WEEK-2026-40",
  "status": "ACTIVE",
  "rulesetId": "challenge-v1",
  "startedAtUtc": "2026-10-04T12:00:00Z",
  "expiresAtUtc": "2026-10-04T12:10:00Z",
  "runTicket": "<signed-run-ticket>"
}
```

**Processing:** Checks challenge window and attempt policy, then creates a bounded run. Attempt limits are not defined yet. `runTicket` is scoped to this player/run/ruleset and is not a substitute for server-side score validation.

### 7.3 `POST /api/v1/challenges/{challenge_id}/runs/{run_id}/submit`

**Status:** Proposed additional endpoint required to separate starting a run from submitting its result.
**Meaning:** Submits a completed challenge run for validation and ranking.
**Authorization:** Player; must own the run.
**Headers:** `Idempotency-Key` required.
**Request:**

```json
{
  "runTicket": "<signed-run-ticket>",
  "clientResult": {
    "grossRevenue": 3200.00,
    "netProfit": 1800.00,
    "earnedStars": 4,
    "durationSeconds": 180
  },
  "eventSequence": []
}
```

`eventSequence` is a proposed compact ordered record of gameplay actions/results; its exact format and anti-cheat sufficiency are not yet defined. Client totals are claims to validate, not authoritative facts.

**Success `202` (proposed):**

```json
{
  "runId": "51137b45-3b42-4d7f-a2c5-41193e6cecd5",
  "status": "PENDING_VALIDATION",
  "submittedAtUtc": "2026-10-04T12:03:00Z"
}
```

An accepted/rejected terminal status is retrieved from the run or leaderboard in the eventual contract. **Open:** whether an additional `GET /runs/{run_id}` is needed and how to validate runs securely.

### 7.4 `GET /api/v1/leaderboards/weekly`

**Meaning:** Reads ranking entries for a weekly challenge.
**Authorization:** Public for public ranking; player identity is shown only as the public display name.
**Query:** `challengeId` required; `limit` optional (default 50, max 100); `cursor` optional.
**Success `200` (proposed):**

```json
{
  "challengeId": "WEEK-2026-40",
  "entries": [
    {
      "rank": 1,
      "displayName": "guest-7ab2",
      "score": 3200.00,
      "submittedAtUtc": "2026-10-04T12:03:00Z"
    }
  ],
  "nextCursor": null,
  "myEntry": {
    "rank": 8,
    "score": 2700.00
  }
}
```

**Processing:** Only validated eligible runs are ranked. Tie-break order is proposed as higher score, then earlier accepted submission. Canonical leaderboard ownership must be one backend service; do not maintain conflicting authoritative scores in both GDevelop and Firebase.

## 8. Proposed real-time competitive match endpoints

### 8.1 `POST /api/v1/matches/queue`

**Meaning:** Enqueues the player for a ranked/unranked competitive match.
**Authorization:** Player.
**Headers:** `Idempotency-Key` required.
**Request:**

```json
{
  "mode": "RANKED_1V1",
  "region": "AUTO",
  "attackCardCodes": [
    "CHARCOAL_SMOKE_BOMB",
    "SAMPLING_NEIGHBOR",
    "MUNICIPAL_INSPECTOR_ALERT"
  ],
  "defenseCardCode": "FIBER_INTERNET"
}
```

`mode` is proposed as `RANKED_1V1`, `CASUAL_1V1`, or `CASUAL_2V2`; support for ranked 2v2 requires a separate decision. Card loadout is optional until sabotage is enabled; if supplied, exactly three distinct attack cards and one defense card are required.

**Success `202` (proposed):**

```json
{
  "queueTicketId": "8ef11078-9396-479a-aafb-f3dfe7632b57",
  "status": "QUEUED",
  "mode": "RANKED_1V1",
  "queuedAtUtc": "2026-10-04T12:00:00Z",
  "estimatedWaitSeconds": 20
}
```

**Processing:** Validate account status and loadout eligibility; match using an approved rating window. Match-found notification is delivered through the chosen realtime channel. Existing upgrades are normalized in competitive modes. MMR formula/region policy is open.

### 8.2 `DELETE /api/v1/matches/queue`

**Meaning:** Cancels the caller's active matchmaking request.
**Authorization:** Player.
**Query:** `queueTicketId` required.
**Success:** `204 No Content`; cancellation is idempotent if already cancelled.
**Failures:** `404` ticket absent/not owned; `409` already matched and queue can no longer be cancelled.

### 8.3 `GET /api/v1/matches/{match_id}`

**Meaning:** Retrieves status and participant-visible state for a match.
**Authorization:** Player participant only.
**Path:** `match_id` — UUID.
**Success `200` (proposed):**

```json
{
  "matchId": "efdb7626-9c4a-47b4-9b13-9bbfab7233d1",
  "mode": "RANKED_1V1",
  "status": "IN_PROGRESS",
  "rulesetId": "pvp-v1",
  "startedAtUtc": "2026-10-04T12:00:00Z",
  "endsAtUtc": "2026-10-04T12:03:00Z",
  "participants": [
    { "participantId": "f2778f60-61b1-40b8-82ad-4ccf6918afef", "displayName": "guest-7ab2", "team": 1 },
    { "participantId": "ef2f53a5-4d9c-4ad3-8b01-588a7d4c2497", "displayName": "guest-a190", "team": 2 }
  ],
  "yourParticipantId": "f2778f60-61b1-40b8-82ad-4ccf6918afef",
  "lastAcceptedSequence": 12,
  "version": 13
}
```

**Processing:** Do not reveal opponent secrets/loadout hidden by match rules. Not found and non-participant are both `404`.

### 8.4 `POST /api/v1/matches/{match_id}/actions`

**Meaning:** Submits a gameplay action in an active match.
**Authorization:** Player participant only.
**Headers:** `Idempotency-Key` required.
**Request:**

```json
{
  "participantSequence": 13,
  "actionCode": "SERVE_ORDER",
  "targetId": "b6f92811-836e-4260-8db5-006e0d5a2d6e",
  "actionData": {
    "preparedSpiceLevel": 5
  },
  "clientOccurredAtUtc": "2026-10-04T12:00:21Z"
}
```

`clientOccurredAtUtc` is diagnostic only; authoritative order and timing come from the match service.

**Success `202` (proposed):**

```json
{
  "matchId": "efdb7626-9c4a-47b4-9b13-9bbfab7233d1",
  "participantSequence": 13,
  "accepted": true,
  "serverReceivedAtUtc": "2026-10-04T12:00:21.120Z",
  "matchVersion": 14
}
```

**Processing:** Validate action against server-side match state, shared order/event seed, participant turn/state, and sequence. The REST endpoint is suitable for discrete authoritative actions; high-frequency animation/state sync should use a separately selected realtime transport rather than trusting client position updates.

### 8.5 `GET /api/v1/matches/{match_id}/result`

**Meaning:** Retrieves the terminal result and rating change of a completed match.
**Authorization:** Player participant only.
**Success `200` (proposed):**

```json
{
  "matchId": "efdb7626-9c4a-47b4-9b13-9bbfab7233d1",
  "status": "COMPLETED",
  "outcome": "WIN",
  "winnerTeam": 1,
  "teamResults": [
    { "team": 1, "netProfit": 1200.00, "ratingChange": 18 },
    { "team": 2, "netProfit": 1050.00, "ratingChange": -18 }
  ],
  "completedAtUtc": "2026-10-04T12:03:00Z"
}
```

`outcome` is `WIN`, `LOSS`, `DRAW`, `FORFEIT`, or `INVALID`. `404` if not visible; `409 MATCH_NOT_FINISHED` before terminal state.

## 9. Proposed PVP sabotage endpoints

### 9.1 `GET /api/v1/sabotage/cards`

**Meaning:** Lists published attack and defense card definitions and their presentation metadata.
**Authorization:** Public.
**Query:** Optional `locale` (default `th-TH`) and `rulesetId`.
**Success `200` (proposed):**

```json
{
  "rulesetId": "pvp-v1",
  "cards": [
    {
      "cardCode": "CHARCOAL_SMOKE_BOMB",
      "kind": "ATTACK",
      "tier": 1,
      "energyCostPct": 25,
      "effectDurationSeconds": 6,
      "counterActionCode": "TOGGLE_EXHAUST_FAN",
      "description": "ควันบดบังสถานีครัวชั่วคราว"
    },
    {
      "cardCode": "FIBER_INTERNET",
      "kind": "DEFENSE",
      "tier": null,
      "energyCostPct": 0,
      "effectDurationSeconds": 0,
      "counterActionCode": null,
      "description": "ป้องกันเหตุการณ์ระบบชำระเงินขัดข้องหนึ่งครั้ง"
    }
  ]
}
```

**Processing:** Catalog only. Costs/effects shown are a proposed ruleset and require balance approval. This draft does not include bribery actions.

### 9.2 `POST /api/v1/matches/{match_id}/sabotage`

**Meaning:** Attempts to spend server-tracked Chaos Meter to apply an attack card to an opposing participant/team.
**Authorization:** Player participant only.
**Headers:** `Idempotency-Key` required.
**Request:**

```json
{
  "participantSequence": 14,
  "cardCode": "CHARCOAL_SMOKE_BOMB",
  "targetParticipantId": "ef2f53a5-4d9c-4ad3-8b01-588a7d4c2497"
}
```

**Success `202` (proposed):**

```json
{
  "matchId": "efdb7626-9c4a-47b4-9b13-9bbfab7233d1",
  "sabotageEventId": "cb104f2d-55d8-42f6-ae75-8bafc4f2e09e",
  "cardCode": "CHARCOAL_SMOKE_BOMB",
  "state": "WARNING",
  "energySpentPct": 25,
  "targetParticipantId": "ef2f53a5-4d9c-4ad3-8b01-588a7d4c2497",
  "serverReceivedAtUtc": "2026-10-04T12:00:22Z"
}
```

**Processing:** Validate participant, target, card in selected loadout, server-calculated meter/cost, match state, immunity, and card restrictions. Meter consumption and event creation are atomic. Client cannot submit its own energy balance or source player identity. `409` insufficient energy, immunity active, invalid target/loadout, or match closed.

### 9.3 `POST /api/v1/matches/{match_id}/sabotage/{event_id}/counter`

**Meaning:** Submits the target participant's response to a sabotage event.
**Authorization:** Target participant only.
**Headers:** `Idempotency-Key` required.
**Request:**

```json
{
  "participantSequence": 15,
  "actionCode": "TOGGLE_EXHAUST_FAN"
}
```

**Success `200` (proposed):**

```json
{
  "matchId": "efdb7626-9c4a-47b4-9b13-9bbfab7233d1",
  "sabotageEventId": "cb104f2d-55d8-42f6-ae75-8bafc4f2e09e",
  "result": "COUNTERED",
  "resolvedAtUtc": "2026-10-04T12:00:24Z",
  "immunityUntilUtc": "2026-10-04T12:00:28Z",
  "matchVersion": 16
}
```

**Processing:** Validate target, allowed counter action, deadline, and unresolved event state. Resolve only once. A successful counter starts the proposed four-second immunity window. Timeout, card-specific counter semantics, defense-slot consumption, and effect stacking are balance/ruleset decisions still open.

## 10. Authentication, confidentiality, and trust boundaries

1. Player operations use the bearer token; a caller must not be able to act on another player's IDs by substituting a path/body ID.
2. Guest/refresh/logout credentials are secrets. Do not place them in URLs, analytics, audit details, or routine logs.
3. Matchmaking, force-trigger, score settlement, wallet updates, and challenge ranking are server-trusted operations. Client-reported timestamps, balances, scores, positions, and player identities are untrusted.
4. Back Office routes remain cookie-authenticated Admin pages and are not exposed through the public API inventory.
5. Production deployments must use HTTPS. If behind a proxy, only trusted forwarded headers may supply client IP.
6. Logs/audit records must avoid storing passwords, access/refresh tokens, full request bodies, or unnecessary personal data.

## 11. Open contract and game-rule decisions

The contract shapes above make missing decisions visible but do not settle them. The following require product/game-rule approval before implementation:

- Whether the first playable release includes only casual gameplay, a weekly leaderboard, real-time 1v1, or 2v2.
- Canonical stage list/config, config publishing/versioning, offline-session behavior, and session restart limits.
- The exact order score, satisfaction-to-stars mapping, star loss/game-over rules, payment/delivery bonus stacking, ingredient deviation scoring, cost basis, repair costs, and pass targets.
- The complete Chaos event registry (the provided registry says eight but currently lists six), trigger cadence, active-event limits, synergy formula/cap, timeout outcomes, and retries.
- Inspector action: original GDD excludes bribery; a lawful official fine versus flee-only response needs a final decision.
- Challenge run verification evidence, attempt count, reward policy, ranking tie-breaks, week boundary, and canonical leaderboard service.
- Match modes, realtime transport, server authority/anti-cheat model, seed handling, MMR/rank formulas, disconnect/forfeit/reconnect behavior, and draw rules.
- Sabotage energy cap/start value, gain stacking, card costs/reuse/cooldowns, defense usage, immunity timing, overlap/team targeting, and normalized upgrade behavior.
- Currency definitions, precision, ledger/retention, paid currency/store receipt validation, and whether any upgrade affects competitive play.
- Public display names/privacy, localization coverage, and data retention for challenge/match histories.

## 12. Change log

| Version | Summary |
|---|---|
| 0.1 | Endpoint inventory: names and meanings only. |
| 0.2 | Added proposed parameters, request/response contracts, authorization, errors, idempotency, processing behavior, and open decisions. |
