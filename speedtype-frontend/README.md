# SpeedType Frontend

A modern Angular application for typing speed tests with real-time feedback and user statistics.

## Features

- **User Authentication**: Registration and login with JWT tokens
- **Real-time Typing Tests**: Character-by-character feedback with live WPM and accuracy
- **Book Management**: Browse public books or create your own typing content
- **User Dashboard**: Personal statistics, recent tests, and quick actions
- **Responsive Design**: Works on desktop and mobile devices
- **Modern UI**: Built with Bootstrap and custom SCSS styling

## Getting Started

### Prerequisites

- Node.js (v20.19+ or v22.12+)
- npm (v8.0+)
- Angular CLI

### Installation

1. Install dependencies:
```bash
npm install
```

2. Start the development server:
```bash
ng serve
```

3. Open your browser and navigate to `http://localhost:4200`

### Backend Setup

Make sure the SpeedType API is running on `http://localhost:5000`. Update the API URLs in the service files if your backend runs on a different port.

## Project Structure

```
src/
├── app/
│   ├── components/          # UI components
│   │   ├── login/          # Login form
│   │   ├── register/       # Registration form
│   │   ├── dashboard/      # User dashboard
│   │   └── typing-test/    # Main typing test interface
│   ├── services/           # API services
│   │   ├── auth.service.ts # Authentication
│   │   ├── book.service.ts # Book management
│   │   └── typing-test.service.ts # Test management
│   ├── models/             # TypeScript interfaces
│   ├── guards/             # Route guards
│   └── interceptors/       # HTTP interceptors
└── styles.scss            # Global styles
```

## Key Components

### Authentication
- JWT-based authentication with automatic token refresh
- Protected routes with guards
- User registration with validation

### Typing Test
- Real-time character-by-character feedback
- Live WPM and accuracy calculation
- Visual feedback for correct/incorrect characters
- Test completion with detailed results

### Dashboard
- User statistics overview
- Recent test history
- Quick access to tests and books
- Performance metrics

## Development

### Building for Production

```bash
ng build --configuration production
```

### Code Quality

The project uses:
- TypeScript strict mode
- Angular best practices
- Reactive programming with RxJS
- Component-based architecture

## API Integration

The frontend communicates with the SpeedType API through dedicated services:

- **AuthService**: User authentication and token management
- **BookService**: Book CRUD operations and searching
- **TypingTestService**: Test creation and statistics

All API calls include automatic JWT token injection via HTTP interceptors.

## Styling

The application uses:
- Bootstrap 5 for base styling
- Custom SCSS for components
- Responsive design principles
- Modern CSS features (Grid, Flexbox)

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)
