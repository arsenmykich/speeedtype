import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BookService } from '../../services/book.service';
import { AuthService } from '../../services/auth.service';
import { Book, User } from '../../models/user.model';

@Component({
  selector: 'app-books',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './books.component.html',
  styleUrls: ['./books.component.scss']
})
export class BooksComponent implements OnInit {
  currentUser: User | null = null;
  allBooks: Book[] = [];
  filteredBooks: Book[] = [];
  searchQuery = '';
  isLoading = true;
  errorMessage = '';

  constructor(
    private bookService: BookService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (!this.currentUser) {
      this.router.navigate(['/login']);
      return;
    }
    this.loadBooks();
  }

  loadBooks(): void {
    this.isLoading = true;
    this.bookService.getAllPublicBooks().subscribe({
      next: (books) => {
        this.allBooks = books;
        this.filteredBooks = books;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Error loading books. Please try again.';
        this.isLoading = false;
        console.error('Error loading books:', error);
      }
    });
  }

  onSearch(): void {
    if (!this.searchQuery.trim()) {
      this.filteredBooks = this.allBooks;
      return;
    }

    this.filteredBooks = this.allBooks.filter(book =>
      book.title.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
      (book.author && book.author.toLowerCase().includes(this.searchQuery.toLowerCase())) ||
      (book.description && book.description.toLowerCase().includes(this.searchQuery.toLowerCase()))
    );
  }

  startTest(book: Book): void {
    this.router.navigate(['/typing-test', book.id]);
  }

  goToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  getWordCount(content: string): number {
    return content ? content.split(' ').length : 0;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
} 