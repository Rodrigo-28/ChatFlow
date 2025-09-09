import { Routes } from '@angular/router';
import { ChatComponent } from './pages/chat/chat.component';
import { HomeComponent } from './pages/home/home.component';
import { WelcomeComponent } from './pages/welcome/welcome.component';
import { PrivateLayoutComponent } from './shared/components/private-layout/private-layout.component';
import { AuthGuard } from './shared/services/auth.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  // { path: 'app', component: ChatComponent },
  {
    path: 'app',
    component: PrivateLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', component: WelcomeComponent },
      { path: 'chat/:id', component: ChatComponent },
    ],
  },
];
