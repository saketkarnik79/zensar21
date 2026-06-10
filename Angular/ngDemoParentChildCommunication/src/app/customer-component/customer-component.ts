import { Component } from '@angular/core';
import { Customer } from '../models/customer.model';
import { Order } from '../models/order.model';
import { CommonModule } from "@angular/common";
import { OrderListComponent } from "../order-list-component/order-list-component";



@Component({
  selector: 'customer',
  imports: [OrderListComponent, CommonModule],
  templateUrl: './customer-component.html',
  styleUrl: './customer-component.css',
})
export class CustomerComponent {
  customers: Customer[] = [
    { id: 1, name: "James", email: "james@gmail.com"},
    { id: 2, name: "Smith", email: "smith@gmail.com"},
    { id: 3, name: "Matthew", email: "matthew@gmail.com"},
  ];
  selectedCustomer?: Customer;
  selectedOrder?: Order;

  selectCustomer(customer: Customer){
    this.selectedCustomer = customer;
    this.selectedOrder = undefined; // resetting order selection
  }

  onOrderSelected(order: Order){
    this.selectedOrder = order;
  }
}
