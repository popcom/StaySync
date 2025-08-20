### ADR-01: Use EF Core for writes, Dapper for reads  (2025-08-17)
- Problem: Balance transaction safety with read performance.
- Options: (A) EF for both, (B) Dapper for both, (C) EF writes + Dapper reads.
- Decision: (C) EF writes + Dapper reads.
- Consequences: Faster projections; clean CQRS split; a bit more plumbing.
- Revisit: If read complexity drops or a new DB appears.

### ADR-02: Store API keys as SHA-256 hash  (2025-08-17)
- Problem: Authenticate hotels safely.
- Options: Plain text, reversible encryption, salted SHA-256.
- Decision: Hash (non-reversible) + constant-time compare.
- Consequences: Must rotate keys via new hash; can’t recover original key.
- Revisit: If we move to OAuth or mTLS.
