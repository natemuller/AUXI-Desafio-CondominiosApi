# UnidadesApi

Consulta as unidades cadastradas (apartamentos, casas ou salas comerciais — tabela `unidades`). Requer token JWT emitido por [`AutenticacaoApi`](autenticacao.md).

- **Porta (dev):** `http://localhost:5399` / `https://localhost:7399`
- **Swagger:** `/swagger`
- **Autenticação:** `Authorization: Bearer <token>` em todos os endpoints
- **Cache:** respostas de sucesso são cacheadas na tabela `cache` (TTL padrão 300s, configurável em `Cache:TtlSeconds`)
- **Paginação:** 10 itens por página (fixo, central em `Core.Common.PaginationDefaults`)

Cada unidade pertence a um bloco (`codBloco`) dentro de um condomínio (`codCondom`), e tem um identificador numérico próprio: `ideconomia`.

> Os query params abaixo não vêm de nenhum documento do desafio — são uma decisão de projeto, pensada para cobrir os filtros mais úteis dado o schema real da tabela.

## GET /api/unidades

Lista unidades com filtros opcionais e paginação.

| Query param | Tipo | Obrigatório | Efeito |
|---|---|---|---|
| `codCondom` | int | não | igualdade exata |
| `codBloco` | string | não | igualdade exata |
| `codEconom` | string | não | igualdade exata |
| `tipoUnidade` | string | não | igualdade exata (ex. apartamento/casa/sala comercial) |
| `ativa` | string | não | igualdade exata (valor bruto da coluna, ex. `"S"`/`"N"`) |
| `nomeCondomino` | string | não | busca parcial, case-insensitive (`ILIKE %nomeCondomino%`) |
| `pagina` | int | não (default `1`) | página < 1 é tratada como 1 |

Exemplo: `GET /api/unidades?codCondom=53&codBloco=A&pagina=1`

### Resposta 200 OK

```json
{
  "items": [
    {
      "ideconomia": 1042,
      "codCondom": 53,
      "codBloco": "A",
      "codEconom": "101",
      "fracao": 0.008333,
      "ativa": "S",
      "dataDesativa": null,
      "dtAlteracao": "2026-02-01T00:00:00",
      "tipoUnidade": "Apartamento",
      "codCondomino": "C-9001",
      "nomeCondomino": "Maria Oliveira",
      "enderecoPrincipal": "Rua das Palmeiras, 100 - Apto 101",
      "enderecoCorrespondencia": null,
      "enderecoCobranca": null,
      "codPesDebConta": null,
      "nomeDebConta": null,
      "codFornec": null,
      "codNaAdmDest": null,
      "codFornecEscrit": null
    }
  ],
  "paginaAtual": 1,
  "itensPorPagina": 10,
  "totalItens": 1,
  "totalPaginas": 1
}
```

Todos os campos da tabela `unidades` são retornados.

## GET /api/unidades/{ideconomia}

Busca uma única unidade pelo seu identificador.

| Path param | Tipo |
|---|---|
| `ideconomia` | int |

- **200 OK** — mesmo objeto do item da listagem acima.
- **404 Not Found** — `ideconomia` não existe.

## Erros comuns

| Status | Quando |
|---|---|
| 401 Unauthorized | header `Authorization` ausente/inválido/expirado |
| 404 Not Found | `GET /api/unidades/{ideconomia}` com id inexistente |
