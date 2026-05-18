import { Component, OnInit, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import OrderService from '../../../services/order-service';

export interface Order {
  id: number;
  customerName: string;
  totalAmount: number;
  createdAt: Date;
}

@Component({
  selector: 'app-order-list',
  templateUrl: './order-list.html',
  styleUrls: ['./order-list.css'],
  imports:[CommonModule]
})
export class OrderListComponent implements OnInit {
  
  orders: Order[] = [];

  constructor(private orderService: OrderService){}

  ngOnInit(): void {
    this.loadOrders();
  }

  ngOnChanges(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    // TODO: Implement order loading logic
    //this.orders = [];
    this.orderService.getOrders().subscribe({next:(data: any) => {
      this.orders = data;
      console.log ('Orders loaded:', this.orders);
    }, error: (err) => {      console.error('Error loading orders:', err);
    }});
  }

  // viewOrder(order: Order): void {
  //   // TODO: Implement view order logic
  //   console.log('View order:', order);
  // }

  // editOrder(order: Order): void {
  //   // TODO: Implement edit order logic
  //   console.log('Edit order:', order);
  // }

  // deleteOrder(order: Order): void {
  //   // TODO: Implement delete order logic
  //   console.log('Delete order:', order);
  // }

  // createOrder(): void {
  //   // TODO: Implement create order logic
  //   console.log('Create new order');
  // }

  // applyFilter(event: Event): void {
  //   const filterValue = (event.target as HTMLInputElement).value;
  //   this.dataSource.filter = filterValue.trim().toLowerCase();
  // }
}
