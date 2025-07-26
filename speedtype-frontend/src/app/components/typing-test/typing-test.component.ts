import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { interval, Subscription } from 'rxjs';
import { BookService, CreateBookRequest } from '../../services/book.service';
import { TypingTestService } from '../../services/typing-test.service';
import { AuthService } from '../../services/auth.service';
import { Book, TypingTest, User } from '../../models/user.model';

@Component({
  selector: 'app-typing-test',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './typing-test.component.html',
  styleUrls: ['./typing-test.component.scss']
})
export class TypingTestComponent implements OnInit, OnDestroy {
  book: Book | null = null;
  currentUser: User | null = null;
  testText = '';
  userInput = '';
  
  // Test selection state
  showSelection = true;
  availableBooks: Book[] = [];
  userBooks: Book[] = [];
  selectedFile: File | null = null;
  uploadForm: FormGroup;
  isUploading = false;
  
  // Test state
  isTestActive = false;
  isTestCompleted = false;
  isLoading = false;
  wasTestStopped = false; // Track if test was manually stopped
  
  // Timing
  startTime: number = 0;
  currentTime: number = 0;
  timerSubscription: Subscription | null = null;
  
  // Statistics
  wpm = 0;
  accuracy = 0;
  errors = 0;
  charactersTyped = 0;
  timeElapsed = 0;
  
  // UI state
  currentWordIndex = 0;
  currentCharIndex = 0;
  words: string[] = [];
  errorMessage = '';
  
  // Test configuration
  wordCount = 50; // Number of words for test
  startWordIndex = 0; // Where to start in the text
  canResumeFromLast = false; // Whether user can resume from last position
  lastCompletedPosition = 0; // Last completed word position
  previewOffset = 0; // For scrolling through the text preview
  testConfiguration = {
    showConfig: false,
    maxWords: 0,
    totalWords: 0
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookService: BookService,
    private typingTestService: TypingTestService,
    private authService: AuthService,
    private fb: FormBuilder
  ) {
    this.uploadForm = this.fb.group({
      title: [''],
      author: [''],
      description: [''],
      isPublic: [false]
    });
  }

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (!this.currentUser) {
      this.router.navigate(['/login']);
      return;
    }

    // Check if a specific book was requested
    const bookId = this.route.snapshot.paramMap.get('bookId');
    if (bookId) {
      this.loadBook(parseInt(bookId));
      this.showSelection = false;
    } else {
      this.loadAvailableBooks();
    }
  }

  ngOnDestroy(): void {
    if (this.timerSubscription) {
      this.timerSubscription.unsubscribe();
    }
  }

  loadAvailableBooks(): void {
    this.isLoading = true;
    
    // Load public books
    this.bookService.getAllPublicBooks().subscribe({
      next: (books) => {
        this.availableBooks = books;
      },
      error: (error) => {
        console.error('Error loading public books:', error);
      }
    });

    // Load user's books
    if (this.currentUser) {
      this.bookService.getBooksByUser(this.currentUser.id).subscribe({
        next: (books) => {
          this.userBooks = books;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading user books:', error);
          this.isLoading = false;
        }
      });
    }
  }

  selectBook(book: Book): void {
    this.isLoading = true;
    // Load full book content including text
    this.bookService.getBookById(book.id).subscribe({
      next: (fullBook) => {
        this.book = fullBook;
        this.initializeTest();
        this.showSelection = false;
        // Show configuration panel when book is selected
        this.testConfiguration.showConfig = true;
      },
      error: (error) => {
        this.errorMessage = 'Error loading book content. Please try again.';
        this.isLoading = false;
        console.error('Error loading book:', error);
      }
    });
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      
      // Auto-fill title from filename
      const filename = file.name.split('.')[0];
      this.uploadForm.patchValue({
        title: filename.charAt(0).toUpperCase() + filename.slice(1)
      });
    }
  }

  uploadFile(): void {
    if (!this.selectedFile || !this.currentUser) {
      this.errorMessage = 'Please select a file and ensure you are logged in.';
      return;
    }

    this.isUploading = true;
    this.errorMessage = '';

    const formData = new FormData();
    formData.append('file', this.selectedFile);
    formData.append('userId', this.currentUser.id.toString());
    
    const formValues = this.uploadForm.value;
    if (formValues.title) formData.append('title', formValues.title);
    if (formValues.author) formData.append('author', formValues.author);
    if (formValues.description) formData.append('description', formValues.description);
    formData.append('isPublic', formValues.isPublic.toString());

    this.bookService.uploadFile(formData).subscribe({
      next: (book) => {
        this.isUploading = false;
        // Uploaded book should already have full content
        this.book = book;
        this.initializeTest();
        this.showSelection = false;
        // Show configuration panel for uploaded book
        this.testConfiguration.showConfig = true;
        this.selectedFile = null;
        this.uploadForm.reset();
      },
      error: (error) => {
        this.isUploading = false;
        this.errorMessage = error.error || 'Error uploading file. Please try again.';
      }
    });
  }

  startRandomTest(): void {
    if (this.availableBooks.length > 0) {
      const randomBook = this.availableBooks[Math.floor(Math.random() * this.availableBooks.length)];
      this.selectBook(randomBook);
    } else {
      this.errorMessage = 'No books available for testing.';
    }
  }

  @HostListener('document:keydown', ['$event'])
  handleKeyDown(event: KeyboardEvent): void {
    // Handle stop test shortcut (Ctrl+S) - works even when test is not active
    if (event.ctrlKey && event.key === 's' && this.isTestActive) {
      event.preventDefault();
      this.stopTest();
      return;
    }

    if (!this.isTestActive || this.isTestCompleted) return;

    // Prevent default behavior for most keys during test
    if (event.key !== 'Tab' && event.key !== 'F5') {
      event.preventDefault();
    }

    if (event.key === 'Backspace') {
      this.handleBackspace();
    } else if (event.key.length === 1) {
      this.handleCharacterInput(event.key);
    }
  }

  loadBook(bookId: number): void {
    this.bookService.getBookById(bookId).subscribe({
      next: (book) => {
        this.book = book;
        this.initializeTest();
      },
      error: (error) => {
        this.errorMessage = 'Error loading book. Please try again.';
        this.isLoading = false;
      }
    });
  }

  initializeTest(): void {
    if (!this.book) return;

    const allWords = this.book.content.split(' ').filter(word => word.trim() !== '');
    this.testConfiguration.totalWords = allWords.length;
    this.testConfiguration.maxWords = Math.min(this.wordCount, allWords.length - this.startWordIndex);
    
    // Extract words from startWordIndex to startWordIndex + wordCount
    this.words = allWords.slice(this.startWordIndex, this.startWordIndex + this.wordCount);
    this.testText = this.words.join(' ');
    
    // Check if user can resume from last position
    this.checkResumeOption();
    
    this.resetTest();
    this.isLoading = false;
  }

  checkResumeOption(): void {
    // Check if user has a previous position saved for this book
    if (this.currentUser && this.book) {
      const savedPosition = localStorage.getItem(`lastPosition_${this.currentUser.id}_${this.book.id}`);
      if (savedPosition) {
        this.lastCompletedPosition = parseInt(savedPosition, 10);
        this.canResumeFromLast = this.lastCompletedPosition > 0;
      }
    }
  }

  resumeFromLastPosition(): void {
    this.startWordIndex = this.lastCompletedPosition;
    this.initializeTest();
  }

  showTestConfiguration(): void {
    this.testConfiguration.showConfig = true;
  }

  applyTestConfiguration(): void {
    // Validate inputs
    if (this.wordCount < 1) this.wordCount = 1;
    if (this.startWordIndex < 0) this.startWordIndex = 0;
    if (this.startWordIndex >= this.testConfiguration.totalWords) {
      this.startWordIndex = Math.max(0, this.testConfiguration.totalWords - this.wordCount);
    }
    
    this.testConfiguration.showConfig = false;
    this.initializeTest();
  }

  cancelConfiguration(): void {
    this.testConfiguration.showConfig = false;
  }

  getPreviewWords(): string[] {
    if (!this.book?.content) return [];
    
    const allWords = this.book.content.split(' ').filter(word => word.trim() !== '');
    // Show 150 words starting from previewOffset
    const start = this.previewOffset;
    const end = Math.min(start + 150, allWords.length);
    return allWords.slice(start, end);
  }

  setStartPosition(relativeIndex: number): void {
    // Convert relative index to absolute index
    const absoluteIndex = this.previewOffset + relativeIndex;
    
    // Ensure the selection doesn't go beyond the text bounds
    const maxStartIndex = Math.max(0, this.testConfiguration.totalWords - this.wordCount);
    this.startWordIndex = Math.min(absoluteIndex, maxStartIndex);
  }

  scrollPreviewForward(): void {
    const maxOffset = Math.max(0, this.testConfiguration.totalWords - 150);
    this.previewOffset = Math.min(this.previewOffset + 50, maxOffset);
  }

  scrollPreviewBackward(): void {
    this.previewOffset = Math.max(0, this.previewOffset - 50);
  }

  resetPreview(): void {
    this.previewOffset = 0;
    this.startWordIndex = 0;
  }

  stopTest(): void {
    if (this.isTestActive) {
      // Ask for confirmation before stopping
      const shouldStop = confirm(
        `Are you sure you want to stop the test?\n\n` +
        `Your progress will be saved:\n` +
        `• Current WPM: ${this.wpm}\n` +
        `• Accuracy: ${this.accuracy.toFixed(1)}%\n` +
        `• Time: ${this.timeElapsed}s\n` +
        `• Progress: ${this.currentWordIndex + 1}/${this.words.length} words`
      );
      
      if (!shouldStop) {
        return; // User cancelled, continue the test
      }
      
      this.isTestActive = false;
      if (this.timerSubscription) {
        this.timerSubscription.unsubscribe();
        this.timerSubscription = null;
      }
      
      // Calculate final stats for partial test
      this.calculateFinalStats();
      
      // Save current position if user typed something
      if (this.currentWordIndex > 0 && this.currentUser && this.book) {
        const currentPosition = this.startWordIndex + this.currentWordIndex;
        localStorage.setItem(`lastPosition_${this.currentUser.id}_${this.book.id}`, currentPosition.toString());
      }
      
      // Save partial test results to database
      if (this.charactersTyped > 0) {
        this.saveTestResult();
      }
      
      // Mark as manually stopped and show results
      this.wasTestStopped = true;
      this.isTestCompleted = true;
    }
  }

  startTest(): void {
    this.isTestActive = true;
    this.startTime = Date.now();
    this.currentTime = this.startTime;
    
    this.timerSubscription = interval(100).subscribe(() => {
      this.currentTime = Date.now();
      this.timeElapsed = Math.floor((this.currentTime - this.startTime) / 1000);
      this.calculateWPM();
    });
  }

  handleCharacterInput(char: string): void {
    if (!this.isTestActive) {
      this.startTest();
    }

    const expectedChar = this.testText[this.userInput.length];
    this.userInput += char;
    this.charactersTyped++;

    if (char !== expectedChar) {
      this.errors++;
    }

    this.updateWordTracking();
    this.calculateStats();

    if (this.userInput.length >= this.testText.length) {
      this.completeTest();
    }
  }

  handleBackspace(): void {
    if (this.userInput.length > 0) {
      this.userInput = this.userInput.slice(0, -1);
      this.updateWordTracking();
      this.calculateStats();
    }
  }

  updateWordTracking(): void {
    const typedWords = this.userInput.split(' ');
    this.currentWordIndex = Math.max(0, typedWords.length - 1);
    
    if (typedWords[this.currentWordIndex]) {
      this.currentCharIndex = typedWords[this.currentWordIndex].length;
    } else {
      this.currentCharIndex = 0;
    }
  }

  calculateStats(): void {
    this.accuracy = this.charactersTyped > 0 
      ? ((this.charactersTyped - this.errors) / this.charactersTyped) * 100 
      : 100;
  }

  calculateWPM(): void {
    const timeInMinutes = this.timeElapsed / 60;
    const wordsTyped = this.userInput.split(' ').length;
    this.wpm = timeInMinutes > 0 ? Math.round(wordsTyped / timeInMinutes) : 0;
  }

  completeTest(): void {
    this.isTestActive = false;
    this.isTestCompleted = true;
    
    if (this.timerSubscription) {
      this.timerSubscription.unsubscribe();
    }

    // Save completed position
    if (this.currentUser && this.book) {
      const completedPosition = this.startWordIndex + this.words.length;
      localStorage.setItem(`lastPosition_${this.currentUser.id}_${this.book.id}`, completedPosition.toString());
    }

    this.calculateFinalStats();
    this.saveTestResult();
  }

  calculateFinalStats(): void {
    const timeInMinutes = this.timeElapsed / 60;
    const wordsTyped = this.userInput.trim().split(' ').length;
    this.wpm = timeInMinutes > 0 ? Math.round(wordsTyped / timeInMinutes) : 0;
    
    const correctChars = this.charactersTyped - this.errors;
    this.accuracy = this.charactersTyped > 0 ? (correctChars / this.charactersTyped) * 100 : 100;
  }

  saveTestResult(): void {
    if (!this.currentUser || !this.book) return;

    const testResult: TypingTest = {
      id: 0,
      wpm: this.wpm,
      accuracy: this.accuracy,
      errors: this.errors,
      time: this.timeElapsed,
      date: new Date(),
      charactersTyped: this.charactersTyped,
      bookId: this.book.id,
      userId: this.currentUser.id
    };

    this.typingTestService.createTest(testResult).subscribe({
      next: (result) => {
        const isPartialTest = this.currentWordIndex < this.words.length;
        console.log(`Test result saved successfully ${isPartialTest ? '(partial test)' : '(completed)'}`);
        
        // Show success message to user
        if (isPartialTest) {
          this.showSuccessMessage(
            `Partial test saved! Progress: ${this.currentWordIndex + 1}/${this.words.length} words, ` +
            `${this.wpm} WPM, ${this.accuracy.toFixed(1)}% accuracy`
          );
        } else {
          this.showSuccessMessage(
            `Test completed! ${this.wpm} WPM, ${this.accuracy.toFixed(1)}% accuracy, ${this.errors} errors`
          );
        }
      },
      error: (error) => {
        console.error('Error saving test result:', error);
        this.showErrorMessage('Failed to save test results. Please try again.');
      }
    });
  }

  private showSuccessMessage(message: string): void {
    // You can replace this with a toast notification or modal
    const element = document.createElement('div');
    element.className = 'alert alert-success position-fixed';
    element.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    element.innerHTML = `<i class="fas fa-check-circle me-2"></i>${message}`;
    document.body.appendChild(element);
    
    setTimeout(() => {
      element.remove();
    }, 4000);
  }

  private showErrorMessage(message: string): void {
    // You can replace this with a toast notification or modal
    const element = document.createElement('div');
    element.className = 'alert alert-danger position-fixed';
    element.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    element.innerHTML = `<i class="fas fa-exclamation-circle me-2"></i>${message}`;
    document.body.appendChild(element);
    
    setTimeout(() => {
      element.remove();
    }, 5000);
  }

  resetTest(): void {
    this.userInput = '';
    this.isTestActive = false;
    this.isTestCompleted = false;
    this.wasTestStopped = false;
    this.currentWordIndex = 0;
    this.currentCharIndex = 0;
    this.wpm = 0;
    this.accuracy = 100;
    this.errors = 0;
    this.charactersTyped = 0;
    this.timeElapsed = 0;
    this.errorMessage = '';
    
    if (this.timerSubscription) {
      this.timerSubscription.unsubscribe();
    }
  }

  restartTest(): void {
    this.resetTest();
  }

  goToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  goToSelection(): void {
    this.showSelection = true;
    this.book = null;
    this.resetTest();
  }

  getCharacterClass(charIndex: number): string {
    if (charIndex < this.userInput.length) {
      const typed = this.userInput[charIndex];
      const expected = this.testText[charIndex];
      return typed === expected ? 'correct' : 'incorrect';
    } else if (charIndex === this.userInput.length) {
      return 'current';
    }
    return '';
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
} 