import { Injectable } from '@angular/core';
//import { Observable } from 'rxjs';
import { Product } from '../models/product';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  getProducts(): Product[] {
    // Implementation for fetching products
    let products: Product[] = [
      { productID: 1, name: 'Product A', price: 10 },
      { productID: 2, name: 'Product B', price: 20 },
      { productID: 3, name: 'Product C', price: 30 },
    ];
    return products;
  }

  getProductById(id: number): Product | undefined {
    // Implementation for fetching a product by ID
    let products: Product[] = this.getProducts();
    return products.find(product => product.productID === id);
  }


}