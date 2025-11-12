import { Component, ElementRef, OnInit } from '@angular/core';
import Pagination from '../../../Utils/Pagination';
import NotaModel from '../../../models/NotaModel';
import { CommonModule, NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import ProdutoModel from '../../../models/ProdutoModel';
import Http from '../../../Utils/Http';
import ProdutoNotaModel from '../../../models/ProdutoNotaModel';

@Component({
  selector: 'notas',
  imports: [NgClass, FormsModule, CommonModule],
  templateUrl: './notas.html',
  styleUrl: './notas.scss',
})
export class Notas implements OnInit {
  page: Pagination = new Pagination();
  notas: NotaModel[] = [];
  info: string = "";
  okAdd: string = "";
  okTimer: NodeJS.Timeout | null = null;
  errorPrint: string = "";
  timeError: NodeJS.Timeout | null = null;
  produtoSearch: string = "";
  produtosProcurados: ProdutoModel[] = [];
  request: boolean = false;

  nota: NotaModel = {
    id: 0,
    aberta: true,
    dataCriacao: new Date(),
    produtos: []
  };

  ngOnInit(): void {
    this.BuscarNotas();
  }

  ChangePage(num: number) {
    switch (num) {
      case -2:
        this.page.Previus();
        break;
      case -1:
        this.page.Previus();
        if (this.page.currentPage == this.page.totalPages - 1)
          this.page.Previus();
        break;
      case 0:
        if (this.page.currentPage == 1)
          this.page.Next();
        else if (this.page.currentPage == this.page.totalPages)
          this.page.Previus();
        break;
      case 1:
        this.page.Next();
        if (this.page.currentPage == 2)
          this.page.Next();
        break;
      case 2:
        this.page.Next();
        break;
    }
    this.BuscarNotas();
  }

  ImprimirNota(id: number){
    const n = this.notas.find(e => e.id == id);
    if(n == null || !n.aberta || this.request)
      return;

    this.request = true;
    this.info = "Imprimindo nota: " + id;
    Http.Post(`http://localhost:3031/NotaFiscal/Imprimir/${id}`)
      .then((_, __) => {
        this.okAdd = "Nota foi impressa.";
        if(this.okTimer != null)
          clearTimeout(this.okTimer);
        this.okTimer = setTimeout(() => this.okAdd = "", 5000);
      })
      .catch(e => {
        this.errorPrint = e;
        if(this.timeError != null)
          clearTimeout(this.timeError);
        this.timeError = setTimeout(() => this.errorPrint = "", 5000);
      })
      .finally(() => {
        this.request = false;
        this.BuscarNotas();
      });
  }

  EnviarNota(){
    if(this.request)
      return;
    this.request = true;

    
    if(this.nota.id == 0){
      this.info = "Registrando nota no sistema.";
      Http.Post("http://localhost:3031/NotaFiscal", this.nota)
      .then((_,__)=>{
        this.okAdd = "Nota adicionada ao sistema.";
        if(this.okTimer != null)
          clearTimeout(this.okTimer);
        this.okTimer = setTimeout(() => this.okAdd = "", 5000);
      })
      .catch(e => {
        this.errorPrint = e;
        if(this.timeError != null)
          clearTimeout(this.timeError);
        this.timeError = setTimeout(() => this.errorPrint = "", 5000);
      })
      .finally(() => {
        this.request = false;
        this.BuscarNotas();
      });
      return;
    }
    
    this.info = "Atualizando nota no sistema.";

    Http.Put("http://localhost:3031/NotaFiscal", this.nota)
    .then((_,__)=>{
      this.okAdd = "Nota atualiza no sistema.";
      if(this.okTimer != null)
        clearTimeout(this.okTimer);
      this.okTimer = setTimeout(() => this.okAdd = "", 5000);
    })
    .catch(e => {
      this.errorPrint = e;
      if(this.timeError != null)
        clearTimeout(this.timeError);
      this.timeError = setTimeout(() => this.errorPrint = "", 5000);
    })
    .finally(() => {
      this.request = false;
      this.BuscarNotas();
    });
  }

  RemoveProduto(cod: string){
    this.nota.produtos = this.nota.produtos.filter(e => e.codigo != cod);
  }

  SelecionarNota(id: number){
    if(id == 0){
      this.nota = {
        id: 0,
        aberta: true,
        dataCriacao: new Date(),
        produtos: []
      };
      return;
    }

    this.nota = this.notas.find(e => e.id == id)!;
  }

  ProcurarProduto(){
    Http.Get("http://localhost:3030/Produto", { name: this.produtoSearch })
      .then<ProdutoModel[]>((e, p) => {
        this.produtosProcurados = e!;
      });
  }

  TemNaNota(cod: string){
    return this.nota.produtos.some(e => e.codigo == cod);
  }

  AdicionarProduto(cod: string){
    if(this.nota.produtos.some(e => e.codigo == cod))
      return;

    const p: ProdutoNotaModel = {
      codigo: cod,
      quantidade: 1
    };

    this.nota.produtos.push(p);
  }

  BuscarNotas(){
    Http.Get("http://localhost:3031/NotaFiscal")
      .then<NotaModel[]>((e, p) => {
        this.notas = e!;
        this.page = p!;
      });
  }
}
