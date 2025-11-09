# SpaceEventHub

Plataforma full-stack para descoberta e gestão de eventos de tecnologia, construída com **MANUS AI** como copiloto criativo de desenvolvimento.

## Sobre o projeto

- Aplicação web com backend em ASP.NET Core 8 e frontend estático em HTML/CSS/JS.
- Banco de dados SQL Server com Entity Framework Core para persistência.
- Foco em facilitar a vida de **organizadores**, **participantes** e **administradores** de eventos.
- Todo o fluxo de ideação, arquitetura e código foi assistido pelo **MANUS AI**, garantindo decisões rápidas e alinhadas.

## Funcionalidades principais

- **Descoberta de eventos** com filtros por cidade, data e trilha.
- **Gestão completa de eventos** para organizadores (criação, edição, cancelamento).
- **Registro de participantes** com confirmação por e-mail e painel pessoal.
- **Notificações em tempo real** e painel analítico para administradores.
- **Relatórios e dashboards** com métricas de uso.

## Stack tecnológica

- **Backend**: ASP.NET Core 8, C# 12, Entity Framework Core, JWT.
- **Frontend**: HTML5, CSS3, JavaScript, Chart.js.
- **Infraestrutura**: Docker, Docker Compose, Nginx.
- **Banco de dados**: SQL Server / Azure SQL.

## Estrutura do repositório

```
SpaceEventHub/
├── Backend/SpaceEventHub.API/    # API ASP.NET Core
├── Frontend/                     # SPA estática
├── Database/                     # Scripts SQL
├── docker-compose.yml            # Orquestração de containers
└── README.md                     # Este documento
```

## Como executar rapidamente

1. Clone o repositório:
   ```bash
   git clone https://github.com/yourusername/spaceeventhub.git
   cd spaceeventhub
   ```
2. Suba tudo com Docker:
   ```bash
   docker-compose up -d
   ```
3. Acesse:
   - Frontend: http://localhost:8080
   - API: http://localhost:5000/api
   - Swagger: http://localhost:5000/swagger

Contas demo:
- Admin: `admin@spaceeventhub.com` / `Admin@123`
- Organizer: `organizer@spaceeventhub.com` / `Organizer@123`
- Attendee: `attendee@spaceeventhub.com` / `Attendee@123`

## Desenvolvimento local

### Backend

```bash
cd Backend/SpaceEventHub.API
dotnet restore
dotnet ef database update
dotnet run
```

A API ficará disponível em `https://localhost:7000`.

### Frontend

Atualize `Frontend/app.js` com a URL da API e sirva os arquivos:

```bash
cd Frontend
python -m http.server 8080
# ou
npx http-server Frontend -p 8080
```

## Créditos

- Criado com suporte do **MANUS AI**, aproveitando arquitetura orientada a componentes, boas práticas de clean code e automação de fluxos.
- UI inspirada em dashboards modernos de tecnologia, com tema escuro e acentos em azul puro.

## Próximos passos sugeridos

- Integrar SignalR para notificações push.
- Adicionar pagamentos e integração com calendários.
- Criar aplicativo mobile com .NET MAUI.

---

**SpaceEventHub** — Conectando pessoas e ideias através de eventos tech, com a inteligência do MANUS AI. 🚀
