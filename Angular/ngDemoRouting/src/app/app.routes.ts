import { Routes } from '@angular/router';
import { HomeComponent } from './home-component/home-component';
import { ContactComponent } from './contact-component/contact-component';
import { ProductComponent } from './product-component/product-component';
import { ErrorComponent } from './error-component/error-component';
import { ProductDetailComponent } from './product-detail-component/product-detail-component';

export const routes: Routes = [
  
  { path: 'home', component: HomeComponent },
  { path: 'contact', component: ContactComponent },
  //{ path: 'products', component: ProductComponent },
//   { path: 'products/:id', component: ProductDetailComponent },
  { path: 'products', component: ProductComponent, 
        children: [
            { path: 'detail/:id', component: ProductDetailComponent }
        ]},

  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: '**', component: ErrorComponent }
];
