# BlocosApi

Consulta os blocos/torres cadastrados (tabela `blocos`). Requer token JWT emitido por [`AutenticacaoApi`](autenticacao.md).

- **Porta (dev):** `http://localhost:5299` / `https://localhost:7299`
- **Swagger:** `/swagger`
- **Autenticação:** `Authorization: Bearer <token>` em todos os endpoints
- **Cache:** respostas de sucesso são cacheadas na tabela `cache` (TTL padrão 300s, configurável em `Cache:TtlSeconds`)
- **Paginação:** 10 itens por página (fixo, central em `Core.Common.PaginationDefaults`)

Um bloco pertence a um condomínio (`codCondom`) e é identificado, dentro dele, por `codBloco` — a chave é composta (`codCondom` + `codBloco`), não existe um ID numérico único de bloco.

> Os query params abaixo não vêm de nenhum documento do desafio (que não especifica Blocos/Torres em detalhe) — são uma decisão de projeto, pensada para cobrir os filtros mais úteis dado o schema real da tabela.

## GET /api/blocos

Lista blocos com filtros opcionais e paginação.

| Query param | Tipo | Obrigatório | Efeito |
|---|---|---|---|
| `codCondom` | int | não | igualdade exata — filtra os blocos de um condomínio |
| `codBloco` | string | não | igualdade exata |
| `descricao` | string | não | busca parcial, case-insensitive (`ILIKE %descricao%`) |
| `ativo` | string | não | igualdade exata (valor bruto da coluna, ex. `"S"`/`"N"`) |
| `pagina` | int | não (default `1`) | página < 1 é tratada como 1 |

Exemplo: `GET /api/blocos?codCondom=53&pagina=1`

### Resposta 200 OK

```json
{
  "items": [
    {
      "codCondom": 53,
      "codBloco": "A",
      "codBlocoBase": null,
      "descricao": "Bloco A - Torre Ipê",
      "qtdEconomias": 40,
      "tipoLograd": "Rua",
      "lograd": "das Palmeiras",
      "numero": "100",
      "bairro": "Centro",
      "cidade": "Belo Horizonte",
      "uf": "MG",
      "cep8Log": "30000000",
      "ativo": "S",
      "tipoBloco": "Residencial"
    }
  ],
  "paginaAtual": 1,
  "itensPorPagina": 10,
  "totalItens": 1,
  "totalPaginas": 1
}
```

Todos os campos da tabela `blocos` são retornados.

## GET /api/blocos/{codCondom}/{codBloco}

Busca um único bloco pela chave composta.

| Path param | Tipo |
|---|---|
| `codCondom` | int |
| `codBloco` | string |

Exemplo: `GET /api/blocos/53/A`

- **200 OK** — mesmo objeto do item da listagem acima.
- **404 Not Found** — combinação `codCondom`/`codBloco` não existe.

## Erros comuns

| Status | Quando |
|---|---|
| 401 Unauthorized | header `Authorization` ausente/inválido/expirado |
| 404 Not Found | `GET /api/blocos/{codCondom}/{codBloco}` com chave inexistente |
