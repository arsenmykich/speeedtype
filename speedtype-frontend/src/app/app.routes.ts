import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { TypingTestComponent } from './components/typing-test/typing-test.component';
import { BooksComponent } from './components/books/books.component';
import { MyBooksComponent } from './components/my-books/my-books.component';
import { LeaderboardComponent } from './components/leaderboard/leaderboard.component';
import { authGuard } from './guards/auth.guard';
import { guestGuard } from './guards/guest.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'login', component: LoginComponent, canActivate: [guestGuard] },
  { path: 'register', component: RegisterComponent, canActivate: [guestGuard] },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'typing-test', component: TypingTestComponent, canActivate: [authGuard] },
  { path: 'typing-test/:bookId', component: TypingTestComponent, canActivate: [authGuard] },
  { path: 'books', component: BooksComponent, canActivate: [authGuard] },
  { path: 'my-books', component: MyBooksComponent, canActivate: [authGuard] },
  { path: 'leaderboard', component: LeaderboardComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: '/dashboard' }
];
