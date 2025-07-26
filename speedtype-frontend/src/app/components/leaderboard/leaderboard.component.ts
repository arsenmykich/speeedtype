import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { TypingTestService } from '../../services/typing-test.service';
import { AuthService } from '../../services/auth.service';
import { TypingTest, User } from '../../models/user.model';

interface LeaderboardEntry {
  rank: number;
  user: User;
  wpm: number;
  accuracy: number;
  testCount: number;
  averageWpm: number;
  averageAccuracy: number;
}

@Component({
  selector: 'app-leaderboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './leaderboard.component.html',
  styleUrls: ['./leaderboard.component.scss']
})
export class LeaderboardComponent implements OnInit {
  currentUser: User | null = null;
  leaderboardData: LeaderboardEntry[] = [];
  topTests: TypingTest[] = [];
  isLoading = true;
  errorMessage = '';
  activeTab = 'wpm'; // 'wpm' or 'accuracy'

  constructor(
    private typingTestService: TypingTestService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (!this.currentUser) {
      this.router.navigate(['/login']);
      return;
    }
    this.loadLeaderboardData();
  }

  loadLeaderboardData(): void {
    this.isLoading = true;
    
    // Load top tests globally
    this.typingTestService.getTopTestsGlobally(10).subscribe({
      next: (tests: TypingTest[]) => {
        this.topTests = tests;
        this.generateLeaderboard(tests);
        this.isLoading = false;
      },
      error: (error: any) => {
        this.errorMessage = 'Error loading leaderboard data. Please try again.';
        this.isLoading = false;
        console.error('Error loading leaderboard:', error);
      }
    });
  }

  generateLeaderboard(tests: TypingTest[]): void {
    // Group tests by user and calculate stats
    const userStats = new Map<number, {
      user: User;
      tests: TypingTest[];
      bestWpm: number;
      bestAccuracy: number;
      totalTests: number;
      averageWpm: number;
      averageAccuracy: number;
    }>();

    tests.forEach(test => {
      if (test.user) {
        const userId = test.user.id;
        if (!userStats.has(userId)) {
          userStats.set(userId, {
            user: test.user,
            tests: [],
            bestWpm: 0,
            bestAccuracy: 0,
            totalTests: 0,
            averageWpm: 0,
            averageAccuracy: 0
          });
        }

        const stats = userStats.get(userId)!;
        stats.tests.push(test);
        stats.bestWpm = Math.max(stats.bestWpm, test.wpm);
        stats.bestAccuracy = Math.max(stats.bestAccuracy, test.accuracy);
        stats.totalTests++;
      }
    });

    // Calculate averages and create leaderboard entries
    const entries: LeaderboardEntry[] = [];
    userStats.forEach((stats, userId) => {
      const totalWpm = stats.tests.reduce((sum, test) => sum + test.wpm, 0);
      const totalAccuracy = stats.tests.reduce((sum, test) => sum + test.accuracy, 0);
      
      entries.push({
        rank: 0, // Will be set when sorting
        user: stats.user,
        wpm: stats.bestWpm,
        accuracy: stats.bestAccuracy,
        testCount: stats.totalTests,
        averageWpm: totalWpm / stats.totalTests,
        averageAccuracy: totalAccuracy / stats.totalTests
      });
    });

    // Sort by best WPM (default)
    this.sortLeaderboard(entries, 'wpm');
    this.leaderboardData = entries;
  }

  sortLeaderboard(entries: LeaderboardEntry[], sortBy: string): void {
    if (sortBy === 'wpm') {
      entries.sort((a, b) => b.wpm - a.wpm);
    } else if (sortBy === 'accuracy') {
      entries.sort((a, b) => b.accuracy - a.accuracy);
    }

    // Assign ranks
    entries.forEach((entry, index) => {
      entry.rank = index + 1;
    });
  }

  switchTab(tab: string): void {
    this.activeTab = tab;
    this.sortLeaderboard(this.leaderboardData, tab);
  }

  getUserRank(userId: number): number {
    const entry = this.leaderboardData.find(e => e.user.id === userId);
    return entry ? entry.rank : 0;
  }

  isCurrentUser(userId: number): boolean {
    return this.currentUser ? this.currentUser.id === userId : false;
  }

  getRankBadgeClass(rank: number): string {
    if (rank === 1) return 'badge-gold';
    if (rank === 2) return 'badge-silver';
    if (rank === 3) return 'badge-bronze';
    return 'badge-default';
  }

  goToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  startTypingTest(): void {
    this.router.navigate(['/typing-test']);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
} 