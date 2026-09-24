# HelpdeskPOO

[![dotnet](https://github.com/brendaalbuq/helpdesk-ticketing-dotnet/actions/workflows/dotnet.yml/badge.svg)](https://github.com/brendaalbuq/helpdesk-ticketing-dotnet/actions/workflows/dotnet.yml)

Trabalho prático individual da UC de Programação Orientada a Objetos da Licenciatura em Engenharia de Sistemas Informáticos (IPCA), no ano letivo 2025/26, desenvolvido em C# e .NET 8.

## Tema

**Helpdesk - Sistema de Gestão de Assistências Técnicas**

O sistema permite gerir clientes, operadores, produtos, problemas conhecidos, tutoriais, tickets de assistência, histórico de interações, resolução, fecho e avaliação do atendimento.

## Estrutura

```text
HelpdeskPOO.sln
src/
  Helpdesk.Core/          Biblioteca principal reutilizável
    Collections/          Repository, FileRepository e TicketPriorityQueue
    Enums/                Estados, prioridades e tipos
    Exceptions/           Exceções customizadas
    Factories/            TicketFactory
    Interfaces/           Contratos
    Logging/              Logs em ficheiro, Singleton e NullLogService
    Models/               Entidades de domínio
    Services/             Serviços, estratégias e observadores
  Helpdesk.ConsoleApp/    Aplicação demonstradora em consola
  Helpdesk.WpfApp/        Interface gráfica WPF
tests/
  Helpdesk.Tests/         Testes unitários NUnit
docs/
  Relatorio.md            Relatório do trabalho
  Relatorio_Helpdesk_POO.pdf  Relatório em PDF
```

## Como executar a consola

```powershell
dotnet restore HelpdeskPOO.sln
dotnet build HelpdeskPOO.sln
dotnet test HelpdeskPOO.sln --collect:"XPlat Code Coverage"
dotnet run --project src/Helpdesk.ConsoleApp/Helpdesk.ConsoleApp.csproj
```

Ao executar a consola, abre um menu interativo para listar clientes, operadores, tickets, abrir novas assistências, atribuir operadores, resolver, fechar e avaliar tickets.

Os dados são gravados em JSON dentro da pasta `Data` do executável, e os logs ficam em `Data/logs/helpdesk.log`.

## Como executar a interface gráfica

```powershell
dotnet run --project src/Helpdesk.WpfApp/Helpdesk.WpfApp.csproj
```

No VS Code, também pode carregar em `F5` e escolher `Executar Helpdesk.WpfApp`.

A interface WPF permite:

- ver contadores do sistema;
- listar tickets;
- registar clientes;
- registar produtos associados a clientes;
- abrir novo ticket;
- atribuir ticket ao primeiro operador adequado;
- marcar como aguardando cliente;
- resolver, fechar e avaliar tickets.

## Pontos da UC cobertos

- Classes e serviços em C# com separação por camadas.
- Biblioteca reutilizável (`Helpdesk.Core`) consumida pela consola e pela interface WPF.
- Interfaces, herança, abstração, encapsulamento e polimorfismo.
- Repositórios genéricos com `Dictionary<Guid, T>` e consultas com LINQ.
- Fila de prioridade com `Dictionary<TicketPriority, Queue<Ticket>>`.
- Persistência em ficheiros JSON.
- Produtos dos clientes e referências dos tickets persistidos por identificadores, com reposição das instâncias canónicas.
- Logs estruturados em ficheiro.
- Exceções customizadas.
- Padrões `Repository`, `Factory`, `Strategy`, `Singleton` e `Observer`.
- 25 testes unitários com NUnit 3.14 e 84,83% de cobertura de linhas no Core.
- CI/CD com GitHub Actions, incluindo compilação, testes e artefactos publicados.
- Interface gráfica WPF simples por cima da biblioteca principal.

## Relatório

O relatório está disponível em [docs/Relatorio.md](docs/Relatorio.md), em formato editável em `output/document/Relatorio_Helpdesk_POO.docx` e em PDF em `output/pdf/Relatorio_Helpdesk_POO.pdf`.
