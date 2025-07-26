import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Book } from '../models/user.model';

export interface CreateBookRequest {
  title: string;
  author?: string;
  description?: string;
  content: string;
  isPublic: boolean;
  userId: number;
}

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private apiUrl = 'http://localhost:5000/api/books';

  constructor(private http: HttpClient) { }

  getAllPublicBooks(): Observable<Book[]> {
    return this.http.get<any[]>(this.apiUrl).pipe(
      map(books => books.map(book => ({
        ...book,
        content: '', // Content not included in list view
        addedAt: new Date(book.addedAt)
      } as Book)))
    );
  }

  getBookById(id: number): Observable<Book> {
    return this.http.get<Book>(`${this.apiUrl}/${id}`);
  }

  getBookWithTests(id: number): Observable<Book> {
    return this.http.get<Book>(`${this.apiUrl}/${id}/with-tests`);
  }

  getBooksByUser(userId: number): Observable<Book[]> {
    return this.http.get<any[]>(`${this.apiUrl}/user/${userId}`).pipe(
      map((books: any[]) => books.map((book: any) => ({
        ...book,
        content: book.content || '', // Content might be included for user's own books
        addedAt: new Date(book.addedAt)
      } as Book)))
    );
  }

  searchBooks(query: string): Observable<Book[]> {
    const params = new HttpParams().set('query', query);
    return this.http.get<Book[]>(`${this.apiUrl}/search`, { params });
  }

  createBook(book: CreateBookRequest): Observable<Book> {
    return this.http.post<Book>(this.apiUrl, book);
  }

  uploadFile(formData: FormData): Observable<Book> {
    return this.http.post<Book>(`${this.apiUrl}/upload`, formData);
  }

  updateBook(id: number, book: CreateBookRequest): Observable<Book> {
    return this.http.put<Book>(`${this.apiUrl}/${id}`, book);
  }

  deleteBook(id: number): Observable<string> {
    return this.http.delete<string>(`${this.apiUrl}/${id}`);
  }
} 