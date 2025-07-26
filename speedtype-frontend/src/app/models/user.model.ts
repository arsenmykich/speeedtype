export interface User {
  id: number;
  username: string;
  email: string;
  password?: string;
  typingTests?: TypingTest[];
  books?: Book[];
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  user: {
    id: number;
    username: string;
    email: string;
  };
}

export interface TypingTest {
  id: number;
  wpm: number;
  accuracy: number;
  errors: number;
  time: number;
  date: Date;
  charactersTyped: number;
  bookId: number;
  userId: number;
  book?: Book;
  user?: User;
}

export interface Book {
  id: number;
  title: string;
  author?: string;
  description?: string;
  imageUrl?: string;
  personalBest: number;
  content: string;
  isPublic: boolean;
  addedAt: Date;
  userId?: number;
  user?: User;
  typingTests?: TypingTest[];
} 