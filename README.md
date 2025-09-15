# ChatFlow — .NET 8 + Angular 18

Aplicación de mensajería **1‑a‑1 en tiempo real** con backend en **.NET 8** (Clean Architecture + WebSockets) y frontend en **Angular 18** (standalone + Angular Material). Permite **registro**, **login con JWT**, **listado de usuarios**, **conversaciones privadas** y **mensajería instantánea** vía **WebSocket**.

> Este repositorio contiene **dos proyectos**:
>
> - **Api/** → Backend (ASP.NET Core Web API + EF Core + JWT + WebSockets)
> - **Web/** → Frontend (Angular 18 + Angular Material + RxJS)

---

## ✨ Características

- **Autenticación segura** con **JWT** (token almacenado en *localStorage*).
- **Usuarios**: registro, login, “me” (perfil) y listado (excluyendo al remitente).
- **Conversaciones privadas** 1‑a‑1 con **mensajes persistentes**.
- **Tiempo real** con **WebSockets**: entrega inmediata de mensajes entrantes al receptor conectado.
- **UI moderna** con Angular Material (layout con *sidenav*, listado de contactos y vista de conversación).
- **Clean Architecture** en el backend (Domain / Application / Infrastructure / Presentation).
- **Swagger/OpenAPI** para probar endpoints (incluye login con Bearer Token).
- **Validaciones** con FluentValidation y **manejo centralizado de errores** en API.

---

## 🗂️ Estructura del repositorio

```
/
├─ Api/                      # Backend (.NET 8 Web API)
│  ├─ Domain/                # Entidades y contratos (User, Role, Conversation, Message, etc.)
│  ├─ Application/           # Casos de uso, DTOs, validaciones, mapeos, WebSocketHandler
│  ├─ Infrastructure/        # EF Core, repositorios, JWT service, BCrypt, Swagger, etc.
│  └─ Presentation/          # Controladores (Auth, Users, Conversations, Messages), middleware
└─ Web/                      # Frontend (Angular 18)
   └─ src/
      ├─ app/
      │  ├─ app.config.ts            # Providers globales (router, http, animations)
      │  ├─ app.routes.ts            # Rutas principales
      │  ├─ core/                    # (según proyecto)
      │  ├─ features/chat/           # Components, services e interfaces de chat
      │  ├─ pages/                   # Home, Chat, Welcome
      │  └─ shared/                  # Layout, servicios (Auth, Cache, WebSocket), guard, etc.
      ├─ environments/               # environment.ts / environment.development.ts
      └─ styles.scss
```

---

## 🧱 Backend (Api/)

**Tecnologías**: ASP.NET Core (.NET 8), **EF Core**, **JWT Bearer**, **BCrypt**, **AutoMapper**, **FluentValidation**, **Swagger**, **System.Net.WebSockets**.

### Arquitectura
- **Domain**: entidades y contratos puros (User, Role, Profile, Conversation, Message, Notification; interfaces de repos/servicios; enum `UserRole`).
- **Application**: casos de uso (Auth, User, Conversation, Message), DTOs, validaciones, mapeos y manejo de tiempo real vía `WebSocketHandler`.
- **Infrastructure**: implementación de repositorios (EF Core), `JwtTokenService`, `PasswordEncryptionService`, configuración de autenticación y Swagger.
- **Presentation**: controladores (`Auth`, `Users`, `Conversations`, `Messages`), middleware de excepciones y de WebSockets, helpers.

### Endpoints (resumen)
- **Auth**
  - `POST /api/Auth/login` → `{ token }`
  - `POST /api/Auth/register` → crea usuario
  - `GET /api/Auth/me` → `{ senderName, senderId }` (requiere `Authorization: Bearer`)
- **Users**
  - `GET /api/Users` → lista todos menos el remitente (según JWT)
- **Conversations**
  - `GET /api/Conversations/{receiverId}` → trae o crea la conversación y marca entrantes como leídos
- **Messages**
  - `POST /api/Messages/send` → persiste y **empuja** por WebSocket si el receptor está online

### Tiempo real (WebSockets)
- Endpoint: `ws://<host>/ws?userId={GUID}`
- El servidor mantiene un registro `userId → WebSocket` en memoria.
- Tras persistir el mensaje, el backend lo **envía** al receptor conectado.

**Ejemplo de evento entrante al cliente**
```json
{
  "conversationId": "GUID...",
  "senderName": "rodrigo",
  "message": "Hola!",
  "sendAt": "2025-09-06T12:35:00Z"
}
```

> **Producción**: Preferir autenticación del WebSocket con **JWT** (query `access_token` o `Sec-WebSocket-Protocol`) y mapear `userId` desde los *claims*. Para múltiples instancias, usar un **backplane** (Redis/RabbitMQ) o migrar a **SignalR**.

### Configuración y ejecución
1) **Variables de entorno / appsettings**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;User Id=...;Password=..."
  },
  "Jwt": {
    "Key": "clave-super-secreta-256bits",
    "Issuer": "chatflow",
    "Audience": "chatflow-client"
  }
}
```
2) **Base de datos**
```bash
cd Api
dotnet ef database update
```
3) **Correr API**
```bash
dotnet run
```
4) **Swagger**
```
https://localhost:<puerto>/swagger
```

---

## 🎨 Frontend (Web/)

**Stack**: **Angular 18** (standalone components + *signals* en `ConversationComponent`), **Angular Material** (tema `azure-blue`), **RxJS 7.8**, **TypeScript 5.5**, **Vite**, `zone.js`.

### Rutas principales
- `/` → `HomeComponent` (contenedor de *Login/Register*)
- `/app` → `PrivateLayoutComponent` (**protegida** por `AuthGuard`)
  - `''` → `WelcomeComponent`
  - `'chat/:id'` → `ChatComponent`

### Flujo de autenticación
1. **Register** (`/auth/register`) → al finalizar hace **login automático** para obtener el **token**.
2. **Login** → guarda `token` en `localStorage` → navega a `/app`.
3. **/app** → `AuthService.getMe()` con `Authorization: Bearer` y cachea `senderId` y `senderName` (vía `CacheService`).
4. **AuthGuard** protege rutas internas verificando que exista `token`.

### Componentes clave
- **HomeComponent**: *toggle* entre *Login* y *Register*.
- **LoginFormComponent**: form reactivo (`email`, `password`) → `AuthService.login()`.
- **RegisterFormComponent**: form reactivo (`email`, `username`, `password`, `confirmPassword`) con validador cruzado `matchPasswordsValidator` → `register → login`.
- **PrivateLayoutComponent**: layout con `mat-sidenav`, carga el perfil y muestra **Contacts** + `router-outlet`.
- **ContactsComponent**: consume `UsersService.getUsers()` (con `Authorization`) y navega a `/app/chat/:id`.
- **ChatComponent**: obtiene `conversation` y abre **WebSocket** con `senderId`; agrega mensajes entrantes si el `conversationId` coincide.
- **ConversationComponent**: usa **signals** para `@Input`/`@Output`; scroll al final; colorea mensajes según `senderId`.
- **UserCardComponent**: muestra `senderName` y permite **logout** (remueve token).

### Servicios principales
- **AuthService**: `login`, `register`, `getMe` (construye `Authorization` manualmente).
- **UsersService**: `getUsers()`.
- **ConversationsService**: `getConversation(userId)`.
- **MessagesService**: `sendMessage(payload)`.
- **CacheService**: `Map` + `BehaviorSubject` para estado compartido reactivo.
- **WebSocketService**: `connect(senderId)` abre `ws://localhost:5125/ws?userId=...` y expone eventos como `Observable<string>`; `disconnect()` cierra el socket.

### Configuración de entorno
`Web/src/environments/environment.development.ts`
```ts
export const environment = {
  baseUrl: 'https://localhost:7105/api', // ← API .NET (desarrollo)
};
```
> Ajustá protocolo/puerto según el backend. Si usás HTTPS de desarrollo, **aceptá el certificado** abriendo Swagger al menos una vez.

### Scripts (Web/package.json)
```json
{
  "start": "ng serve",
  "build": "ng build",
  "watch": "ng build --watch --configuration development",
  "test": "ng test",
  "format": "prettier --write \"src/**/*.{ts,html,scss}\""
}
```

---

## 🚀 Puesta en marcha (local)

1. **API**  
   ```bash
   cd Api
   dotnet ef database update   # si aplica
   dotnet run
   ```
   Verificar Swagger en `https://localhost:<puerto>/swagger` y aceptar el certificado.

2. **Web**  
   ```bash
   cd Web
   npm install
   # Revisar Web/src/environments/environment.development.ts
   npm start
   ```
   Abrir `http://localhost:4200`

3. **Probar flujo**  
   - Registrarse → se hará **login automático** → navegar a `/app`
   - En `/app`, se precarga el perfil y se listan **Contacts**
   - Abrir un chat y enviar mensajes; los entrantes llegan por **WebSocket**

---

## 🩺 Troubleshooting

- **401 en `/auth/me` o `/Users`**  
  - Confirmar que el `token` exista en `localStorage`.
  - Verificar que las llamadas incluyan `Authorization: Bearer <token>` (el código lo arma manualmente).
  - Revisar `environment.baseUrl` (protocolo/puerto correctos) y aceptar el certificado de desarrollo.

- **500 en `/Users`**  
  - Endpoints `[Authorize]`: enviar token. Validar que el backend decodifique el identificador de usuario correctamente.

- **WebSocket no conecta**  
  - Ajustar **host/puerto/protocolo** en `WebSocketService` (`ws://localhost:5125/ws?...`).
  - Si el backend corre con HTTPS, considerar `wss://` con un certificado confiable.

- **Angular “Outdated Optimize Dep / EPERM rename” (Windows)**  
  - Cerrar `ng serve`, borrar `.angular/cache` y volver a `npm start`.

---

## 📈 Recomendaciones de evolución

- Autenticar el **handshake de WebSocket con JWT** (en lugar de `userId` por query).
- Soportar **múltiples conexiones por usuario**.
- **Backplane** (Redis/RabbitMQ) o **SignalR** para escalar tiempo real en múltiples instancias.
- **Paginación con cursor** en mensajes (`sent_at`) y **índices** en DB.
- Tests unitarios e integración (servicios, repos y controladores).

---




