### ✅ Instruction for Coding Assistant

---

#### **Development Focus**
I am a software architect working with .NET 8.0+, ASP.NET MVC, Razor Pages, Blazor (Server & WASM), HTMX, and Vue.js. I build enterprise-grade applications using clean architecture, SOLID principles, DDD (Domain-Driven Design), CQRS, and REPR (Request → Endpoint → Response with Request Handler Tests).

For data access, I use Entity Framework Core and Dapper, focusing on performance, maintainability, and correct separation of concerns.

---

#### **Coding Standards & Best Practices**
- Follow official **.NET coding conventions** and **C# 12 best practices**.
- Adhere strictly to **SOLID principles**, including:
  - **Single Responsibility Principle** – each class should have only one reason to change.
  - **Open/Closed Principle** – entities should be open for extension, but closed for modification.
  - **Liskov Substitution Principle** – subclasses must be substitutable for their base types.
  - **Interface Segregation Principle** – use multiple, specific interfaces rather than a large, general-purpose one.
  - **Dependency Inversion Principle** – depend on abstractions, not concrete classes.
- Use **async/await** everywhere for non-blocking I/O. Avoid `Result`, `Wait`, or `GetAwaiter().GetResult()`.
- Use **DI (Dependency Injection)** consistently. Don’t create `new` instances of services manually.
- Use **minimal APIs** or **REPR** when building APIs, emphasizing endpoint simplicity and testability.

---

#### **Frontend Patterns**
- In **Vue/HTMX**: structure components for maintainability and reusability. Follow separation of HTML, CSS, and JS logic (preferably use TypeScript in Vue).
- In **Blazor**: use MVU (Model-View-Update) architecture and **Cascading Parameters** responsibly. Ensure state management is centralized and predictable.

---

#### **Testing & Quality Assurance**
- Write unit and integration tests using **xUnit** and **Shouldly**.
- Use **WebApplicationFactory** for ASP.NET Core API integration tests.
- Follow the **AAA (Arrange, Act, Assert)** pattern.
- Favor **test isolation** using **Moq** or **FakeItEasy**, and use **bUnit** for Blazor component tests.
- Apply **TDD** where practical.
- Use **Playwright** or **Cypress** for E2E testing of Vue/HTMX apps.

---

#### **Data Layer Instructions**
**Do not modify domain models, DTOs, database schemas, or data access code** unless explicitly instructed. All data access logic (EF/Dapper) should be **minimal, optimized, and wrapped in service/repository abstractions**. Use explicit mappings when transforming models.

---

#### **AI Code Generation & Review Preferences**
- All generated code must be **.NET 8+ compliant** and respect **SOLID**, **REPR**, and **clean architecture** principles.
- **Do not refactor existing domain models, services, or repositories** unless asked to.
- Provide **explanations** for non-trivial logic, especially:
  - LINQ queries
  - Async flows
  - Complex mappings
  - Performance-sensitive data access
- Suggest **optimizations** only after the initial implementation is complete.
- Clearly call out trade-offs and alternatives when multiple solutions are possible.

---

#### **Infrastructure & DevOps Expectations**
- Assume **CI/CD pipelines with GitHub Actions or Jenkins**.
- Secure APIs using **OAuth2**, **OpenID Connect**, and **claims-based auth** (Okta or IdentityServer).
- Include **EF Core migrations**, Dapper scripts, or proper SQL migrations as applicable.
- For hosting, optimize **Kestrel/IIS** settings, middleware, and HTTPS redirects.

---

#### **General AI Assistant Guidance**
- Prioritize **real-world**, **production-ready** code over theoretical examples.
- Avoid outdated or deprecated APIs.
- Focus on **clarity, correctness, and maintainability** over conciseness.
- Only suggest external libraries that are actively maintained and compatible with .NET 8+.
- Respect architectural boundaries; avoid merging responsibilities or over-engineering solutions.

---

