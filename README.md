# SpeedType - Typing Speed Test Application

A modern, full-stack typing speed test application built with Angular frontend and .NET Core API backend. Practice your typing skills with customizable tests, track your progress, and compete on the leaderboard.

## 🚀 Features

### Core Functionality
- **Real-time Typing Tests**: Practice with customizable word counts and starting positions
- **Progress Tracking**: Save and resume tests mid-way with Ctrl+S shortcut
- **Performance Analytics**: Track WPM, accuracy, errors, and completion time
- **Personal Best Records**: Maintain your highest scores for each text
- **User Authentication**: Secure login/register system with JWT tokens

### Content Management
- **Public Text Library**: Access a curated collection of typing texts
- **Personal Text Upload**: Upload your own texts for practice
- **Text Customization**: Set word count, starting position, and test parameters
- **Text Visibility**: Make your texts public or keep them private

### Social Features
- **Global Leaderboard**: Compete with other users on WPM and accuracy
- **User Profiles**: View your typing statistics and recent tests
- **Progress Dashboard**: Visual overview of your typing performance

### User Experience
- **Responsive Design**: Works seamlessly on desktop and mobile devices
- **Modern UI**: Clean, minimalistic interface with smooth animations
- **Keyboard Shortcuts**: Quick actions for power users (Ctrl+S to stop test)
- **Real-time Feedback**: Instant visual feedback during typing tests

## 📸 Screenshots

### Authentication
[Screenshot: Login Page]
- Clean login form with email/password authentication
- Responsive design with validation feedback

[Screenshot: Registration Page]
- User registration with username, email, and password
- Real-time validation and error handling

### Dashboard
[Screenshot: Main Dashboard]
- Overview of user statistics (Average WPM, Accuracy, Tests Completed)
- Quick action buttons for navigation
- Recent tests display with performance metrics

### Typing Test
[Screenshot: Test Selection]
- Choose from public texts or personal uploads
- Text preview with word count and difficulty indicators

[Screenshot: Test Configuration]
- Customize word count (1-500 words)
- Select starting position with visual text preview
- Resume from last position option

[Screenshot: Active Typing Test]
- Real-time typing interface with character highlighting
- Live statistics display (WPM, accuracy, time, errors)
- Progress tracking and completion status

[Screenshot: Test Results]
- Detailed performance summary
- Comparison with personal bests
- Option to retry or try different texts

### Content Management
[Screenshot: Books Library]
- Browse public texts with search functionality
- Filter by author, title, or description
- Quick access to start tests

[Screenshot: My Books]
- Personal text management
- Upload, edit, and delete functionality
- Public/private visibility controls

### Leaderboard
[Screenshot: Global Leaderboard]
- Top performers by WPM and accuracy
- User rankings with detailed statistics
- Tab navigation between different metrics

## 🛠️ Technology Stack

### Frontend
- **Angular 17**: Modern reactive framework
- **TypeScript**: Type-safe development
- **SCSS**: Advanced styling with CSS variables
- **Bootstrap 5**: Responsive UI components
- **RxJS**: Reactive programming for state management

### Backend
- **.NET 9**: Latest .NET framework
- **Entity Framework Core**: ORM for database operations
- **PostgreSQL**: Reliable database system
- **JWT Authentication**: Secure token-based auth
- **BCrypt**: Password hashing and verification

### Development Tools
- **Angular CLI**: Development and build tools
- **Entity Framework Migrations**: Database schema management
- **Swagger/OpenAPI**: API documentation
- **Git**: Version control

## 📋 Prerequisites

Before running the application, ensure you have the following installed:

- **Node.js** (v18 or higher)
- **npm** (comes with Node.js)
- **.NET 9 SDK**
- **PostgreSQL** (v12 or higher)
- **Git**

## 🚀 Installation & Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd speedtype
```

### 2. Backend Setup

#### Install Dependencies
```bash
cd speedtype.API
dotnet restore
```

#### Configure Database
1. Create a PostgreSQL database named `speedtype`
2. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=speedtype;Username=your_username;Password=your_password"
  }
}
```

#### Run Database Migrations
```bash
dotnet ef database update
```

#### Configure JWT Settings
Update `appsettings.json` with your JWT configuration:
```json
{
  "Jwt": {
    "Key": "your-super-secret-jwt-key-here-minimum-16-characters",
    "Issuer": "SpeedTypeAPI",
    "Audience": "SpeedTypeAPI"
  }
}
```

#### Start the API Server
```bash
dotnet run
```
The API will be available at `http://localhost:5000`

### 3. Frontend Setup

#### Install Dependencies
```bash
cd speedtype-frontend
npm install
```

#### Start the Development Server
```bash
npm start
```
The application will be available at `http://localhost:4200`

### 4. Quick Start Script
Use the provided script to start both servers:
```bash
./run-app.sh
```

## 🎯 Usage Guide

### Getting Started
1. **Register an Account**: Create your profile with username, email, and password
2. **Browse Texts**: Explore the public library or upload your own texts
3. **Start Testing**: Choose a text and configure your test parameters
4. **Track Progress**: Monitor your performance on the dashboard
5. **Compete**: Check the leaderboard and improve your rankings

### Typing Test Features
- **Word Count**: Select 1-500 words for your test
- **Starting Position**: Choose where to begin in longer texts
- **Resume Functionality**: Continue from where you left off
- **Real-time Stats**: See WPM, accuracy, and errors as you type
- **Keyboard Shortcuts**: Use Ctrl+S to stop and save progress

### Content Management
- **Upload Texts**: Add your own content for practice
- **Public/Private**: Control visibility of your uploaded texts
- **Search & Filter**: Find texts by title, author, or description
- **Performance Tracking**: Monitor your best scores for each text

## 🔧 Configuration

### Environment Variables
Create a `.env` file in the frontend directory:
```env
API_BASE_URL=http://localhost:5000
```

### Database Configuration
The application uses PostgreSQL with Entity Framework Core. Key tables:
- **Users**: User accounts and authentication
- **Books**: Text content and metadata
- **TypingTests**: Test results and performance data

### API Endpoints
- `POST /api/auth/login` - User authentication
- `POST /api/auth/register` - User registration
- `GET /api/books` - Retrieve public texts
- `POST /api/books` - Upload new texts
- `GET /api/typing-tests` - Get test results
- `POST /api/typing-tests` - Save test results

## 🧪 Testing

### Backend Tests
```bash
cd speedtype.API
dotnet test
```

### Frontend Tests
```bash
cd speedtype-frontend
npm test
```

## 📦 Deployment

### Production Build
```bash
# Frontend
cd speedtype-frontend
npm run build

# Backend
cd speedtype.API
dotnet publish -c Release
```

### Docker Support
```bash
# Build and run with Docker Compose
docker-compose up --build
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

If you encounter any issues:
1. Check the [Issues](https://github.com/your-repo/issues) page
2. Create a new issue with detailed description
3. Include error logs and system information

## 🔮 Roadmap

- [ ] **Advanced Analytics**: Detailed performance insights and trends
- [ ] **Text Categories**: Organize texts by difficulty and genre
- [ ] **Multiplayer Mode**: Real-time typing competitions
- [ ] **Mobile App**: Native iOS and Android applications
- [ ] **API Rate Limiting**: Enhanced security and performance
- [ ] **Export Results**: Download test results and statistics
- [ ] **Custom Themes**: Personalize the interface appearance
- [ ] **Text-to-Speech**: Audio support for accessibility

## 📊 Performance Metrics

- **Typing Speed**: Words per minute (WPM) calculation
- **Accuracy**: Character-level error tracking
- **Progress Tracking**: Resume functionality for interrupted tests
- **Statistics**: Historical performance analysis
- **Leaderboards**: Global and personal rankings

---

**Built with ❤️ using Angular and .NET Core**

*SpeedType - Improve your typing skills, one test at a time.* 