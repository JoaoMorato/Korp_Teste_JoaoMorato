import { Component, signal } from '@angular/core';
import { Produtos } from './view/produtos/produtos';
import { Notas } from './view/notas/notas';

@Component({
  selector: 'app-root',
  imports: [Produtos, Notas],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('Korp-Site');
  prod_nota = true;

  ChangeTab(){
    this.prod_nota = !this.prod_nota;
  }
}
