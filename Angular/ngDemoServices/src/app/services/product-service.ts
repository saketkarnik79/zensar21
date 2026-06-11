import { Injectable } from '@angular/core';
import { Product } from '../models/product';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  public getProducts(): Product[] {
    let products: Product[];

    products = [
      { productID: 1, name: 'Memory Card', price: 5000 },
      { productID: 2, name: 'Pen Drive', price: 750 },
      { productID: 3, name: 'Power Bank', price: 2000 }
    ];

    return products;
  }
}
