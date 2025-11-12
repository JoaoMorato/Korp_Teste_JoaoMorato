import ProdutoNotaModel from "./ProdutoNotaModel";

export default interface NotaModel {
    id: number,
    dataCriacao: Date,
    dataFechamento?: Date,
    aberta: Boolean,
    produtos: ProdutoNotaModel[]
}