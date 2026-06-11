import { Component, OnInit, signal } from '@angular/core';
import { User } from '../models/user';
import { UserService } from '../services/userService';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-users',
  imports: [FormsModule, CommonModule],
  templateUrl: './users.html',
  styleUrl: './users.css',
})
export class Users implements OnInit {
  // users: User[] = [];
  users = signal<User[]>([]);

  selectedUser: User = { name: '', email: '' };
  isEditMode: boolean = false;

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.userService.getUsers().subscribe((data) => {
      // this.users = data;
      this.users.set(data); // replacing entire users array
    });
  }

  saveUser(): void {
    if(this.isEditMode){
      this.userService.updateUser(this.selectedUser.id!, this.selectedUser).subscribe(() => {
        this.resetForm();
        this.loadUsers();
      });
    } else {
      this.userService.createUser(this.selectedUser).subscribe(() => {
        this.resetForm();
        this.loadUsers();
      });
    }
  }

  editUser(user: User): void {
    this.selectedUser = { ...user };
    this.isEditMode = true;
  }
  
  deleteUser(id?: number): void {
    if(!id) return;
    this.userService.deleteUser(id).subscribe(() => {
      this.loadUsers();
    });
  }

  resetForm(): void {
    this.selectedUser = { name: '', email: '' };
    this.isEditMode = false;
  }
}
