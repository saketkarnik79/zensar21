import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { Customer } from '../models/customer.model';
import { Order } from '../models/order.model';
import { CommonModule } from "@angular/common";

@Component({
  selector: 'order-list',
  imports: [CommonModule],
  templateUrl: './order-list-component.html',
  styleUrl: './order-list-component.css',
})
export class OrderListComponent implements OnChanges {
  @Input() customer!: Customer; // Parent -> Child
  @Output() orderSelected = new EventEmitter<Order>(); // Child -> Parent

  orders: Order[] = [
    {id: 101, productName: 'Laptop', amount: 70000, customerId: 1},
    {id: 102, productName: 'Mouse', amount: 500, customerId: 1},
    {id: 103, productName: 'Phone', amount: 50000, customerId: 3},
    {id: 104, productName: 'Monitor', amount: 7000, customerId: 2},
  ];

  customerOrders: Order[]=[];

  ngOnChanges(): void {
    //Filter Order whenever customer selection changes
    this.customerOrders = this.orders.filter(order => order.customerId === this.customer.id);
  }

  selectOrder(order: Order){
    this.orderSelected.emit(order);
  }
} 
