# CondominiosApi

Consulta os condomínios cadastrados (tabela `condominios`). Requer token JWT emitido por [`AutenticacaoApi`](autenticacao.md).

- **Porta (dev):** `http://localhost:5216` / `https://localhost:7236`
- **Swagger:** `/swagger`
- **Autenticação:** `Authorization: Bearer <token>` em todos os endpoints
- **Cache:** respostas de sucesso são cacheadas na tabela `cache` (TTL padrão 300s, configurável em `Cache:TtlSeconds`)
- **Paginação:** 10 itens por página (fixo, central em `Core.Common.PaginationDefaults`)

## GET /api/condominios

Lista condomínios com filtros opcionais e paginação.

| Query param | Tipo | Obrigatório | Efeito |
|---|---|---|---|
| `cnpj` | string | não | igualdade exata |
| `codCondom` | int | não | igualdade exata |
| `nome` | string | não | busca parcial, case-insensitive (`ILIKE %nome%`) |
| `pagina` | int | não (default `1`) | página < 1 é tratada como 1 |

Exemplo: `GET /api/condominios?nome=jardim&pagina=1`

### Resposta 200 OK

```json
{
  "items": [
    {
      "codCondom": 53,
      "nomeCondom": "Residencial Jardim das Flores",
      "ativo": "S",
      "cnpj": "12345678000199",
      "cei": null,
      "inscrMunicip": null,
      "qtdBlocos": 3,
      "qtdUnidades": 120,
      "totalFracao": 100.0,
      "diaVencDoc": 10,
      "dataInicioAdm": 20180101,
      "dataDistrato": null,
      "motivoDistrato": null,
      "assessor": "Fulano de Tal",
      "filial": "01",
      "agencia": "0001",
      "sindico": "Ciclano da Silva",
      "subSindico": null,
      "conselheiro": null,
      "gestor": "Beltrano",
      "conselhoFiscal": null,
      "conselhoConsultivo": null,
      "conselhoSuplente": null,
      "tipoCondominio": "Residencial",
      "tipoCategoria": null,
      "dtAlteracao": "2026-01-15T00:00:00",
      "tipoLograd": "Rua",
      "lograd": "das Palmeiras",
      "numero": "100",
      "bairro": "Centro",
      "cidade": "Belo Horizonte",
      "cep8Log": "30000000",
      "uf": "MG",
      "codPessoaSindico": null,
      "nomeSindico": "Ciclano da Silva",
      "cpfDocnpj": null,
      "condGarantido": null,
      "tipoConta": null,
      "obsCobranca": null,
      "garantidora": null
    }
  ],
  "paginaAtual": 1,
  "itensPorPagina": 10,
  "totalItens": 1,
  "totalPaginas": 1
}
```

Todos os campos da tabela `condominios` são retornados — sem contrato "curado", conforme pedido.

## GET /api/condominios/{codCondom}

Busca um único condomínio pela chave primária.

| Path param | Tipo |
|---|---|
| `codCondom` | int |

- **200 OK** — mesmo objeto do item da listagem acima.
- **404 Not Found** — `codCondom` não existe.

## Erros comuns

| Status | Quando |
|---|---|
| 401 Unauthorized | header `Authorization` ausente/inválido/expirado |
| 404 Not Found | `GET /api/condominios/{codCondom}` com código inexistente |
