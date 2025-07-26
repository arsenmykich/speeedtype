import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BookService } from '../../services/book.service';
import { AuthService } from '../../services/auth.service';
import { Book, User } from '../../models/user.model';

@Component({
  selector: 'app-my-books',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './my-books.component.html',
  styleUrls: ['./my-books.component.scss']
})
export class MyBooksComponent implements OnInit {
  currentUser: User | null = null;
  userBooks: Book[] = [];
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
    this.loadUserBooks();
  }

  loadUserBooks(): void {
    if (!this.currentUser) return;
    
    this.isLoading = true;
    this.bookService.getBooksByUser(this.currentUser.id).subscribe({
      next: (books) => {
        this.userBooks = books;
        this.filteredBooks = books;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Error loading your books. Please try again.';
        this.isLoading = false;
        console.error('Error loading user books:', error);
      }
    });
  }

  onSearch(): void {
    if (!this.searchQuery.trim()) {
      this.filteredBooks = this.userBooks;
      return;
    }

    this.filteredBooks = this.userBooks.filter(book =>
      book.title.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
      (book.author && book.author.toLowerCase().includes(this.searchQuery.toLowerCase())) ||
      (book.description && book.description.toLowerCase().includes(this.searchQuery.toLowerCase()))
    );
  }

  startTest(book: Book): void {
    this.router.navigate(['/typing-test', book.id]);
  }

  editBook(book: Book): void {
    // TODO: Implement edit functionality
    console.log('Edit book:', book);
  }

  deleteBook(book: Book): void {
    if (confirm(`Are you sure you want to delete "${book.title}"?`)) {
      this.bookService.deleteBook(book.id).subscribe({
        next: () => {
          this.userBooks = this.userBooks.filter(b => b.id !== book.id);
          this.onSearch(); // Re-filter the displayed books
        },
        error: (error) => {
          this.errorMessage = 'Error deleting book. Please try again.';
          console.error('Error deleting book:', error);
        }
      });
    }
  }

  toggleVisibility(book: Book): void {
    const updatedBook = { ...book, isPublic: !book.isPublic };
    // TODO: Implement update functionality
    console.log('Toggle visibility for book:', updatedBook);
  }

  uploadNewBook(): void {
    this.router.navigate(['/typing-test']);
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