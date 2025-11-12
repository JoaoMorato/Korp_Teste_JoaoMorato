import { Component, OnInit } from '@angular/core';
import Pagination from '../../../Utils/Pagination';
import ProdutoModel from '../../../models/ProdutoModel';
import { FormsModule } from '@angular/forms';
import { NgClass } from '@angular/common';
import Http from '../../../Utils/Http';

@Component({
  selector: 'produtos',
  imports: [FormsModule, NgClass,],
  templateUrl: './produtos.html',
  styleUrl: './produtos.scss',
})
export class Produtos implements OnInit {
  page: Pagination = new Pagination();
  produtos: ProdutoModel[] = [];
  okAdd: string = "";
  okTimer: NodeJS.Timeout | null = null;
  errorAdd: string = "";
  errorTimer: NodeJS.Timeout | null = null;
  info: string = "";
  request: boolean = false;
  validate = {
    cod: false,
    desc: false,
    saldo: false
  }

  produtoData: ProdutoModel = {
    novo: true,
    codigo: "",
    descricao: "",
    saldo: 1
  };

  ngOnInit(): void {
    this.BuscarProdutos();
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
    this.BuscarProdutos();
  }

  SelectProduto(prod?: ProdutoModel) {
    if (prod != null) {
      this.produtoData = prod;
      this.produtoData.novo = false;
    } else {
      this.produtoData = {
        novo: true,
        codigo: "",
        descricao: "",
        saldo: 1
      };
    }
  }

  BuscarProdutos() {
    Http.Get<ProdutoModel[]>("http://localhost:3030/Produto", { name: "", pagination: this.page })
      .then<ProdutoModel[]>((e, p) => {
        this.produtos = e!;
        this.page = p!;
      });
  }

  AdicionarProduto() {
    this.produtoData.saldo = Math.floor(this.produtoData.saldo);
    this.validate.cod = this.produtoData.codigo.length == 0;
    this.validate.desc = this.produtoData.descricao.length == 0;
    this.validate.saldo = this.produtoData.saldo <= 0;

    if (!this.validate.cod && !this.validate.desc && !this.validate.saldo) {
      this.request = true;
      if(this.produtoData.novo){
        this.info = "Adicionando produto ao sistema.";  
        Http.Post<ProdutoModel>("http://localhost:3030/Produto", this.produtoData)
          .then((_, __) => {
            this.okAdd = "Produto adicionado.";
            if (this.okTimer != null)
              clearTimeout(this.okTimer);
            this.okTimer = setTimeout(() => this.okAdd = "", 5000);
          })
          .catch(e => {
            this.errorAdd = e;
            if (this.errorTimer != null)
              clearTimeout(this.errorTimer);
            this.errorTimer = setTimeout(() => this.errorAdd = "", 5000);
          })
          .finally(() => {
            this.request = false;
            this.BuscarProdutos();
          });
      }else{
        this.info = "Atualizando produto no sistema.";
        Http.Put<ProdutoModel>("http://localhost:3030/Produto", this.produtoData)
          .then((_, __) => {
            this.okAdd = "Produto atualizado.";
            if (this.okTimer != null)
              clearTimeout(this.okTimer);
            this.okTimer = setTimeout(() => this.okAdd = "", 5000);
          })
          .catch(e => {
            this.errorAdd = e;
            if (this.errorTimer != null)
              clearTimeout(this.errorTimer);
            this.errorTimer = setTimeout(() => this.errorAdd = "", 5000);
          })
          .finally(() => {
            this.request = false;
            this.BuscarProdutos();
          });
      }
    }
  }

  ChangeProdutoCod(event: KeyboardEvent) {
    if (event.key == "Backspace") {
      if (this.produtoData.codigo.length > 0)
        this.produtoData.codigo = this.produtoData.codigo.substring(0, this.produtoData.codigo.length - 1);
      return;
    }
    if (this.produtoData.codigo.length == 10 || !/^[A-Za-z0-9]$/.test(event.key))
      return;
    this.produtoData.codigo += event.key.toUpperCase();
  }

}
