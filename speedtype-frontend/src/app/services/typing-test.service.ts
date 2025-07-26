import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TypingTest } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class TypingTestService {
  private apiUrl = 'http://localhost:5000/api/typingtests';

  constructor(private http: HttpClient) { }

  getAllTests(): Observable<TypingTest[]> {
    return this.http.get<TypingTest[]>(this.apiUrl);
  }

  getTestById(id: number): Observable<TypingTest> {
    return this.http.get<TypingTest>(`${this.apiUrl}/${id}`);
  }

  getTestsByUser(userId: number): Observable<TypingTest[]> {
    return this.http.get<TypingTest[]>(`${this.apiUrl}/user/${userId}`);
  }

  getTestsByBook(bookId: number): Observable<TypingTest[]> {
    return this.http.get<TypingTest[]>(`${this.apiUrl}/book/${bookId}`);
  }

  getRecentTests(userId: number, count: number = 10): Observable<TypingTest[]> {
    const params = new HttpParams().set('count', count.toString());
    return this.http.get<TypingTest[]>(`${this.apiUrl}/user/${userId}/recent`, { params });
  }

  getLeaderboard(count: number = 10): Observable<TypingTest[]> {
    const params = new HttpParams().set('count', count.toString());
    return this.http.get<TypingTest[]>(`${this.apiUrl}/leaderboard`, { params });
  }

  getTopTestsGlobally(count: number = 10): Observable<TypingTest[]> {
    const params = new HttpParams().set('count', count.toString());
    return this.http.get<TypingTest[]>(`${this.apiUrl}/leaderboard`, { params });
  }

  getBestTest(userId: number, bookId: number): Observable<TypingTest> {
    return this.http.get<TypingTest>(`${this.apiUrl}/user/${userId}/best/${bookId}`);
  }

  getUserStats(userId: number): Observable<{ userId: number; averageWPM: number; averageAccuracy: number }> {
    return this.http.get<{ userId: number; averageWPM: number; averageAccuracy: number }>(`${this.apiUrl}/user/${userId}/stats`);
  }

  createTest(test: TypingTest): Observable<TypingTest> {
    return this.http.post<TypingTest>(this.apiUrl, test);
  }

  updateTest(id: number, test: TypingTest): Observable<TypingTest> {
    return this.http.put<TypingTest>(`${this.apiUrl}/${id}`, test);
  }

  deleteTest(id: number): Observable<string> {
    return this.http.delete<string>(`${this.apiUrl}/${id}`);
  }
} 