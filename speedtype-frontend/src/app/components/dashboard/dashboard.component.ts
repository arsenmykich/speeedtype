import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { TypingTestService } from '../../services/typing-test.service';
import { User, TypingTest } from '../../models/user.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  currentUser: User | null = null;
  userStats: any = null;
  recentTests: TypingTest[] = [];
  isLoading = true;

  constructor(
    private authService: AuthService,
    private typingTestService: TypingTestService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });

    this.loadUserData();
  }

  loadUserData(): void {
    this.isLoading = true;
    
    // Load recent tests
    this.typingTestService.getRecentTests(5).subscribe({
      next: (tests) => {
        this.recentTests = tests;
        this.calculateUserStats();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading recent tests:', error);
        this.isLoading = false;
      }
    });
  }

  calculateUserStats(): void {
    if (this.recentTests.length === 0) {
      this.userStats = {
        averageWPM: 0,
        averageAccuracy: 0
      };
      return;
    }

    const totalWPM = this.recentTests.reduce((sum, test) => sum + test.wpm, 0);
    const totalAccuracy = this.recentTests.reduce((sum, test) => sum + test.accuracy, 0);
    
    this.userStats = {
      averageWPM: Math.round(totalWPM / this.recentTests.length),
      averageAccuracy: Math.round((totalAccuracy / this.recentTests.length) * 100) / 100
    };
  }

  startTypingTest(): void {
    this.router.navigate(['/typing-test']);
  }

  goToBooks(): void {
    this.router.navigate(['/books']);
  }

  goToMyBooks(): void {
    this.router.navigate(['/my-books']);
  }

  goToLeaderboard(): void {
    this.router.navigate(['/leaderboard']);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString();
  }
} 