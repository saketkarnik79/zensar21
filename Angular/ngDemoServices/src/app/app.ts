import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Product } from './models/product';
import { ProductService } from './services/product-service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('ngDemoServices');
  
  products?: Product[];
  //productService?: ProductService;

  //Tight coupling
  // constructor(){ 
  //   this.productService = new ProductService();
  // }

  //Loose coupling with dependency injection
  constructor(private productService: ProductService){
    //this.productService = productService;
  }

  getProducts() {
    this.products = this.productService?.getProducts();
  }
}
