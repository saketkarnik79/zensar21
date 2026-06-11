import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-product-detail-component',
  imports: [],
  templateUrl: './product-detail-component.html',
  styleUrl: './product-detail-component.css',
})
export class ProductDetailComponent {
  productID: string = '';

  constructor(private route: ActivatedRoute) {
    this.route.params.subscribe((params) => {
      this.productID = params['id'];
    });
  }
}
