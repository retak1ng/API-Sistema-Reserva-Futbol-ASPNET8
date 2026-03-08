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