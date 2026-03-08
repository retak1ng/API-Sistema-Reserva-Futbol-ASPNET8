🚀 Etapa 1 completada: Setup profesional de una API con Clean Architecture

Lo que hice hoy no fue escribir features — fue construir
los cimientos para que el proyecto escale.

📐 Decisiones técnicas tomadas:
→ 4 proyectos separados (Domain, Application, Infrastructure, API)
→ Las referencias entre capas siguen la Dependency Rule de Uncle Bob
→ Domain sin ningún NuGet externo (cero acoplamiento)
→ CQRS con MediatR desde el diseño inicial

La diferencia entre un proyecto de portfolio amateur y uno
profesional empieza en el día 1, antes de escribir lógica.

-----------------------------------------------------------------------

🧱 Etapa 2: Construí el Domain Layer de mi API de Reservas

Sin frameworks. Sin NuGets. C# puro.

Decisiones técnicas que tomé:

→ Factory Methods en lugar de constructores públicos
  Las entidades solo pueden crearse en estado válido.
  Es imposible tener un User sin email o una Availability
  con fecha pasada.

→ Result Pattern en lugar de excepciones
  "Turno ya reservado" no es una excepción — es un flujo
  esperado. Lo manejé con Result<T> para hacerlo explícito.

→ Errores de dominio tipados
  DomainErrors.Availability.AlreadyBooked en lugar de
  throw new Exception("ya reservado"). Buscable, testeable,
  sin magic strings.

El Domain no sabe que existe ASP.NET ni Firebase.
Eso es Clean Architecture en la práctica.

-----------------------------------------------------------------

⚙️ Etapa 3: Application Layer — donde viven los casos de uso

Construí el corazón de la lógica de mi API de Reservas:

→ CQRS con MediatR
  Separé operaciones de escritura (Commands) de lectura
  (Queries). Cada caso de uso es una clase independiente.

→ Pipeline Behaviors
  Logging y Validación se ejecutan automáticamente antes
  de cada Handler. Sin repetir código. Patrón Decorator.

→ Seguridad en Login
  El mismo error para "usuario no existe" y "contraseña
  incorrecta". Nunca revelar cuál falló.

→ Contratos sin implementación
  La Application define interfaces (IUserRepository,
  IJwtTokenGenerator). No sabe nada de Firebase ni JWT.

La Infrastructure implementará esos contratos en la
próxima etapa.

--------------------------------------------------------------------

🔧 Etapa 4: Infrastructure Layer — Firebase + JWT implementados

Hoy conecté el sistema al mundo real sin tocar el dominio:

→ Firebase Firestore como base de datos NoSQL
  Mapeo manual Firestore ↔ Entidades de dominio.
  Sin ORM — el dominio permanece completamente limpio.

→ JWT con claims tipados
  El token lleva Id, Email, Name y Role del usuario.
  Firmado con HMAC-SHA256.

→ Settings fuertemente tipados
  JwtSettings y FirebaseSettings mapeados desde
  appsettings.json. Sin magic strings en el código.

→ Lifetimes correctos en DI
  FirebaseContext → Singleton (una sola conexión)
  Repositorios → Scoped (por request HTTP)
  JwtTokenGenerator → Singleton (stateless)

La Application no sabe nada de esto.
Solo conoce interfaces. Eso es desacoplamiento real.