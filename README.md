================================================================================ FRONTEND FILES (Angualar 18 & Typescript)
frontend/package.json : this comes with the app creation ──────────────────────────────────────────────────────────────────────────── { "name": "wonga-frontend", "version": "1.0.0", "private": true, "dependencies": { "angular": "^18.2.0", "react-dom": "^18.2.0", "angualr-router-dom": "^6.20.0", "axios": "^1.6.0", "angular-scripts": "5.0.1" }, "scripts": { "start": "react-scripts start", "build": "react-scripts build", "test": "react-scripts test", "eject": "angular-scripts eject" }, "eslintConfig": { "extends": [ "angular-app" ] }, "browserslist": { "development": [ "last 1 chrome version", "last 1 firefox version", "last 1 safari version" ] } }

────────────────────────────────────────────────────────────────────────────

frontend/.env this comes with the app creation ──────────────────────────────────────────────────────────────────────────── REACT_APP_API_URL=http://localhost:5000/api

────────────────────────────────────────────────────────────────────────────

frontend/public/index.html ────────────────────────────────────────────────────────────────────────────

<title>Wonga Assessment</title> You need to enable JavaScript to run this app.
────────────────────────────────────────────────────────────────────────────

frontend/src/index.js ──────────────────────────────────────────────────────────────────────────── import React from 'react'; import ReactDOM from 'react-dom/client'; import './styles/index.css'; import App from './App';

const root = ReactDOM.createRoot(document.getElementById('root')); root.render( <React.StrictMode> </React.StrictMode> );

────────────────────────────────────────────────────────────────────────────

frontend/src/App.js ──────────────────────────────────────────────────────────────────────────── import React from 'react'; import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom'; import { AuthProvider, useAuth } from './context/AuthContext'; import Register from './pages/Register'; import Login from './pages/Login'; import UserDetails from './pages/UserDetails'; import ProtectedRoute from './components/ProtectedRoute'; import './styles/app.css';

const AppContent = () => { const { isAuthenticated, isLoading } = useAuth();

if (isLoading) { return

Loading...
; }
return ( <Route path="/register" element={isAuthenticated ? : } /> <Route path="/login" element={isAuthenticated ? : } /> <Route path="/user-details" element={ } /> <Route path="/" element={} /> <Route path="*" element={} /> ); };

function App() { return ( ); }

export default App;

────────────────────────────────────────────────────────────────────────────

frontend/src/context/AuthContext.js ──────────────────────────────────────────────────────────────────────────── import React, { createContext, useState, useContext, useEffect } from 'react';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => { const [user, setUser] = useState(null); const [token, setToken] = useState(null); const [isLoading, setIsLoading] = useState(true);

useEffect(() => { // Load auth state from localStorage on mount const savedToken = localStorage.getItem('token'); const savedUser = localStorage.getItem('user');

if (savedToken && savedUser) {
  setToken(savedToken);
  setUser(JSON.parse(savedUser));
}

setIsLoading(false);
}, []);

const login = (userData, authToken) => { setUser(userData); setToken(authToken); localStorage.setItem('token', authToken); localStorage.setItem('user', JSON.stringify(userData)); };

const logout = () => { setUser(null); setToken(null); localStorage.removeItem('token'); localStorage.removeItem('user'); };

const value = { user, token, isLoading, login, logout, isAuthenticated: !!token, };

return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>; };

export const useAuth = () => { const context = useContext(AuthContext); if (!context) { throw new Error('useAuth must be used within an AuthProvider'); } return context; };

────────────────────────────────────────────────────────────────────────────

frontend/src/services/api.js this is connecting to the api ──────────────────────────────────────────────────────────────────────────── import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

const api = axios.create({ baseURL: API_BASE_URL, headers: { 'Content-Type': 'application/json', }, });

// Add token to requests api.interceptors.request.use( (config) => { const token = localStorage.getItem('token'); if (token) { config.headers.Authorization = Bearer ${token}; } return config; }, (error) => Promise.reject(error) );

export const authService = { register: (data) => api.post('/auth/register', data), login: (data) => api.post('/auth/login', data), getUserDetails: () => api.get('/user/details'), };

export default api;

────────────────────────────────────────────────────────────────────────────

frontend/src/components/ProtectedRoute.js ──────────────────────────────────────────────────────────────────────────── import React from 'react'; import { Navigate } from 'react-router-dom'; import { useAuth } from '../context/AuthContext';

const ProtectedRoute = ({ children }) => { const { isAuthenticated, isLoading } = useAuth();

if (isLoading) { return

Loading...
; }
if (!isAuthenticated) { return ; }

return children; };

export default ProtectedRoute;

────────────────────────────────────────────────────────────────────────────

frontend/src/pages/Register.js ──────────────────────────────────────────────────────────────────────────── import React, { useState } from 'react'; import { useNavigate, Link } from 'react-router-dom'; import { authService } from '../services/api'; import '../styles/auth.css';

const Register = () => { const [formData, setFormData] = useState({ firstName: '', lastName: '', email: '', password: '', }); const [error, setError] = useState(''); const [loading, setLoading] = useState(false); const navigate = useNavigate();

const handleChange = (e) => { const { name, value } = e.target; setFormData((prev) => ({ ...prev, [name]: value, })); };

const handleSubmit = async (e) => { e.preventDefault(); setError(''); setLoading(true);

try {
  const response = await authService.register({
    firstName: formData.firstName,
    lastName: formData.lastName,
    email: formData.email,
    password: formData.password,
  });

  if (response.data.success) {
    navigate('/login', { state: { message: 'Registration successful! Please log in.' } });
  } else {
    setError(response.data.message);
  }
} catch (err) {
  setError(err.response?.data?.message || 'Registration failed. Please try again.');
} finally {
  setLoading(false);
}
};

return (

Create Account
{error &&
{error}
}
First Name
Last Name
Email
Password
{loading ? 'Creating Account...' : 'Sign Up'}
Already have an account? Log in

); };
export default Register;

────────────────────────────────────────────────────────────────────────────

frontend/src/pages/Login.js ──────────────────────────────────────────────────────────────────────────── import React, { useState, useEffect } from 'react'; import { useNavigate, Link, useLocation } from 'react-router-dom'; import { authService } from '../services/api'; import { useAuth } from '../context/AuthContext'; import '../styles/auth.css';

const Login = () => { const [formData, setFormData] = useState({ email: '', password: '', }); const [error, setError] = useState(''); const [successMessage, setSuccessMessage] = useState(''); const [loading, setLoading] = useState(false); const navigate = useNavigate(); const location = useLocation(); const { login } = useAuth();

useEffect(() => { // Display success message from registration if (location.state?.message) { setSuccessMessage(location.state.message); } }, [location]);

const handleChange = (e) => { const { name, value } = e.target; setFormData((prev) => ({ ...prev, [name]: value, })); };

const handleSubmit = async (e) => { e.preventDefault(); setError(''); setLoading(true);

try {
  const response = await authService.login({
    email: formData.email,
    password: formData.password,
  });

  if (response.data.success) {
    login(response.data.user, response.data.token);
    navigate('/user-details');
  } else {
    setError(response.data.message);
  }
} catch (err) {
  setError(err.response?.data?.message || 'Login failed. Please try again.');
} finally {
  setLoading(false);
}
};

return (

Log In
{successMessage &&
{successMessage}
} {error &&
{error}
}
Email
Password
{loading ? 'Logging In...' : 'Log In'}
Don't have an account? Sign up

); };
export default Login;

────────────────────────────────────────────────────────────────────────────

frontend/src/pages/UserDetails.js ──────────────────────────────────────────────────────────────────────────── import React, { useState, useEffect } from 'react'; import { useNavigate } from 'react-router-dom'; import { authService } from '../services/api'; import { useAuth } from '../context/AuthContext'; import '../styles/user-details.css';

const UserDetails = () => { const [userData, setUserData] = useState(null); const [loading, setLoading] = useState(true); const [error, setError] = useState(''); const navigate = useNavigate(); const { user, logout } = useAuth();

useEffect(() => { const fetchUserDetails = async () => { try { const response = await authService.getUserDetails(); if (response.data.success) { setUserData(response.data.data); } else { setError(response.data.message); } } catch (err) { setError(err.response?.data?.message || 'Failed to fetch user details'); } finally { setLoading(false); } };

fetchUserDetails();
}, []);

const handleLogout = () => { logout(); navigate('/login'); };

if (loading) { return

Loading user details...
; }
if (error) { return (

{error}
Back to Login
); }
return (

Welcome, {userData?.firstName}!
Log Out
    <div className="details-section">
      <h2>Your Profile Information</h2>
      <div className="detail-item">
        <label>First Name:</label>
        <p>{userData?.firstName}</p>
      </div>
      <div className="detail-item">
        <label>Last Name:</label>
        <p>{userData?.lastName}</p>
      </div>
      <div className="detail-item">
        <label>Email Address:</label>
        <p>{userData?.email}</p>
      </div>
    </div>

    <div className="info-box">
      <p>This page is only accessible to authenticated users. Your data is securely retrieved from the backend API.</p>
    </div>
  </div>
</div>
); };

export default UserDetails;

────────────────────────────────────────────────────────────────────────────

frontend/src/styles/index.css ────────────────────────────────────────────────────────────────────────────

{ margin: 0; padding: 0; box-sizing: border-box; }
body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue', sans-serif; -webkit-font-smoothing: antialiased; -moz-osx-font-smoothing: grayscale; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 100vh; display: flex; align-items: center; justify-content: center; }

code { font-family: source-code-pro, Menlo, Monaco, Consolas, 'Courier New', monospace; }

#root { width: 100%; min-height: 100vh; }

.loading { display: flex; justify-content: center; align-items: center; min-height: 100vh; font-size: 18px; color: #fff; font-weight: 500; }

.loading-screen { display: flex; justify-content: center; align-items: center; min-height: 100vh; font-size: 24px; color: #fff; font-weight: 600; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); }

────────────────────────────────────────────────────────────────────────────

frontend/src/styles/app.css ──────────────────────────────────────────────────────────────────────────── .App { min-height: 100vh; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); }

.loading-screen { display: flex; justify-content: center; align-items: center; min-height: 100vh; font-size: 24px; color: white; font-weight: 600; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); }

────────────────────────────────────────────────────────────────────────────

frontend/src/styles/auth.css ──────────────────────────────────────────────────────────────────────────── .auth-container { display: flex; justify-content: center; align-items: center; min-height: 100vh; padding: 20px; }

.auth-box { background: white; border-radius: 10px; box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2); padding: 40px; width: 100%; max-width: 400px; animation: slideIn 0.3s ease-in-out; }

@keyframes slideIn { from { opacity: 0; transform: translateY(-20px); } to { opacity: 1; transform: translateY(0); } }

.auth-box h2 { color: #333; margin-bottom: 30px; text-align: center; font-size: 28px; font-weight: 600; }

.form-group { margin-bottom: 20px; }

.form-group label { display: block; margin-bottom: 8px; color: #555; font-weight: 500; font-size: 14px; }

.form-group input { width: 100%; padding: 12px; border: 1px solid #ddd; border-radius: 5px; font-size: 14px; transition: border-color 0.3s ease; }

.form-group input:focus { outline: none; border-color: #667eea; box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1); }

.submit-btn { width: 100%; padding: 12px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; border: none; border-radius: 5px; font-size: 16px; font-weight: 600; cursor: pointer; transition: transform 0.2s ease, box-shadow 0.2s ease; }

.submit-btn:hover:not(:disabled) { transform: translateY(-2px); box-shadow: 0 5px 20px rgba(102, 126, 234, 0.4); }

.submit-btn:disabled { opacity: 0.7; cursor: not-allowed; }

.error-message { background-color: #fee; border: 1px solid #fcc; color: #c33; padding: 12px; border-radius: 5px; margin-bottom: 20px; font-size: 14px; text-align: center; }

.success-message { background-color: #efe; border: 1px solid #cfc; color: #3c3; padding: 12px; border-radius: 5px; margin-bottom: 20px; font-size: 14px; text-align: center; }

.auth-link { text-align: center; margin-top: 20px; color: #666; font-size: 14px; }

.auth-link a { color: #667eea; text-decoration: none; font-weight: 600; transition: color 0.2s ease; }

.auth-link a:hover { color: #764ba2; }

────────────────────────────────────────────────────────────────────────────

frontend/src/styles/user-details.css ──────────────────────────────────────────────────────────────────────────── .user-details-container { display: flex; justify-content: center; align-items: center; min-height: 100vh; padding: 20px; }

.user-details-box { background: white; border-radius: 10px; box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2); padding: 40px; width: 100%; max-width: 500px; animation: slideIn 0.3s ease-in-out; }

@keyframes slideIn { from { opacity: 0; transform: translateY(-20px); } to { opacity: 1; transform: translateY(0); } }

.header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 40px; border-bottom: 2px solid #f0f0f0; padding-bottom: 20px; }

.header h1 { color: #333; font-size: 24px; font-weight: 600; }

.logout-btn { padding: 10px 20px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; border: none; border-radius: 5px; font-size: 14px; font-weight: 600; cursor: pointer; transition: transform 0.2s ease, box-shadow 0.2s ease; white-space: nowrap; }

.logout-btn:hover { transform: translateY(-2px); box-shadow: 0 5px 20px rgba(102, 126, 234, 0.4); }

.details-section { margin-bottom: 30px; }

.details-section h2 { color: #333; font-size: 18px; font-weight: 600; margin-bottom: 20px; border-bottom: 1px solid #e0e0e0; padding-bottom: 10px; }

.detail-item { margin-bottom: 20px; padding: 15px; background: #f9f9f9; border-radius: 5px; border-left: 4px solid #667eea; }

.detail-item label { display: block; color: #666; font-size: 12px; font-weight: 600; text-transform: uppercase; margin-bottom: 5px; letter-spacing: 0.5px; }

.detail-item p { color: #333; font-size: 16px; font-weight: 500; word-break: break-all; }

.info-box { background: linear-gradient(135deg, rgba(102, 126, 234, 0.1) 0%, rgba(118, 75, 162, 0.1) 100%); border-left: 4px solid #667eea; padding: 15px; border-radius: 5px; margin-top: 20px; }

.info-box p { color: #555; font-size: 14px; line-height: 1.6; }

.loading { display: flex; justify-content: center; align-items: center; min-height: 100vh; font-size: 18px; color: #666; font-weight: 500; }

.error-message { background-color: #fee; border: 1px solid #fcc; color: #c33; padding: 15px; border-radius: 5px; margin-bottom: 20px; font-size: 14px; text-align: center; }

────────────────────────────────────────────────────────────────────────────

frontend/Dockerfile ──────────────────────────────────────────────────────────────────────────── FROM node:18-alpine AS build

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

ENV REACT_APP_API_URL=http://localhost:5000/api

RUN npm run build

FROM node:18-alpine

WORKDIR /app

RUN npm install -g serve

COPY --from=build /app/build ./build

EXPOSE 3000

CMD ["serve", "-s", "build", "-l", "3000"]

────────────────────────────────────────────────────────────────────────────

────────────────────────────────────────────────────────────────────────────

AuthenticationServiceTests.cs ──────────────────────────────────────────────────────────────────────────── using Moq; using Xunit; using FluentAssertions; using WongaAPI.DTOs; using WongaAPI.Models; using WongaAPI.Repositories; using WongaAPI.Services;

namespace WongaAPI.Tests.Services;

public class AuthenticationServiceTests { private readonly Mock _mockUserRepository; private readonly Mock _mockTokenService; private readonly AuthenticationService _authService;

public AuthenticationServiceTests()
{
    _mockUserRepository = new Mock<IUserRepository>();
    _mockTokenService = new Mock<ITokenService>();
    _authService = new AuthenticationService(_mockUserRepository.Object, _mockTokenService.Object);
}

#region Register Tests

[Fact]
public async Task RegisterAsync_ValidRequest_ReturnsSuccess()
{
    // Arrange
    var request = new RegisterRequest
    {
        FirstName = "John",
        LastName = "Doe",
        Email = "john@example.com",
        Password = "Password123"
    };

    var user = new User
    {
        Id = 1,
        FirstName = request.FirstName,
        LastName = request.LastName,
        Email = request.Email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        CreatedAt = DateTime.UtcNow
    };

    _mockUserRepository.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>()))
        .ReturnsAsync((User?)null);
    _mockUserRepository.Setup(r => r.CreateUserAsync(It.IsAny<User>()))
        .ReturnsAsync(user);

    // Act
    var result = await _authService.RegisterAsync(request);

    // Assert
    result.Success.Should().BeTrue();
    result.Data.Should().NotBeNull();
    result.Data!.Email.Should().Be(request.Email.ToLower());
    _mockUserRepository.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Once);
}

[Fact]
public async Task RegisterAsync_DuplicateEmail_ReturnsFail()
{
    // Arrange
    var request = new RegisterRequest
    {
        FirstName = "John",
        LastName = "Doe",
        Email = "john@example.com",
        Password = "Password123"
    };

    var existingUser = new User
    {
        Id = 1,
        FirstName = "Jane",
        LastName = "Doe",
        Email = "john@example.com",
        PasswordHash = "hash"
    };

    _mockUserRepository.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>()))
        .ReturnsAsync(existingUser);

    // Act
    var result = await _authService.RegisterAsync(request);

    // Assert
    result.Success.Should().BeFalse();
    result.Message.Should().Contain("already registered");
    _mockUserRepository.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Never);
}

[Fact]
public async Task RegisterAsync_ShortPassword_ReturnsFail()
{
    // Arrange
    var request = new RegisterRequest
    {
        FirstName = "John",
        LastName = "Doe",
        Email = "john@example.com",
        Password = "short"
    };

    // Act
    var result = await _authService.RegisterAsync(request);

    // Assert
    result.Success.Should().BeFalse();
    result.Message.Should().Contain("at least 6 characters");
}

[Fact]
public async Task RegisterAsync_MissingFirstName_ReturnsFail()
{
    // Arrange
    var request = new RegisterRequest
    {
        FirstName = "",
        LastName = "Doe",
        Email = "john@example.com",
        Password = "Password123"
    };

    // Act
    var result = await _authService.RegisterAsync(request);

    // Assert
    result.Success.Should().BeFalse();
    result.Message.Should().Contain("First name is required");
}

#endregion

#region Login Tests

[Fact]
public async Task LoginAsync_ValidCredentials_ReturnsToken()
{
    // Arrange
    var request = new LoginRequest
    {
        Email = "john@example.com",
        Password = "Password123"
    };

    var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
    var user = new User
    {
        Id = 1,
        FirstName = "John",
        LastName = "Doe",
        Email = "john@example.com",
        PasswordHash = passwordHash
    };

    _mockUserRepository.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>()))
        .ReturnsAsync(user);
    _mockTokenService.Setup(t => t.GenerateToken(It.IsAny<User>()))
        .Returns("fake-jwt-token");

    // Act
    var result = await _authService.LoginAsync(request);

    // Assert
    result.Success.Should().BeTrue();
    result.Token.Should().NotBeNullOrEmpty();
    result.User.Should().NotBeNull();
    result.User!.Id.Should().Be(user.Id);
}

[Fact]
public async Task LoginAsync_InvalidPassword_ReturnsFail()
{
    // Arrange
    var request = new LoginRequest
    {
        Email = "john@example.com",
        Password = "WrongPassword"
    };

    var passwordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
    var user = new User
    {
        Id = 1,
        FirstName = "John",
        LastName = "Doe",
        Email = "john@example.com",
        PasswordHash = passwordHash
    };

    _mockUserRepository.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>()))
        .ReturnsAsync(user);

    // Act
    var result = await _authService.LoginAsync(request);

    // Assert
    result.Success.Should().BeFalse();
    result.Message.Should().Contain("Invalid email or password");
}

[Fact]
public async Task LoginAsync_UserNotFound_ReturnsFail()
{
    // Arrange
    var request = new LoginRequest
    {
        Email = "nonexistent@example.com",
        Password = "Password123"
    };

    _mockUserRepository.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>()))
        .ReturnsAsync((User?)null);

    // Act
    var result = await _authService.LoginAsync(request);

    // Assert
    result.Success.Should().BeFalse();
    result.Message.Should().Contain("Invalid email or password");
}

#endregion
}

────────────────────────────────────────────────────────────────────────────

================================================================================ CONFIGURATION FILES
docker-compose.yml ──────────────────────────────────────────────────────────────────────────── version: '3.8'

services: postgres: image: postgres:16-alpine container_name: wonga-postgres environment: POSTGRES_USER: postgres POSTGRES_PASSWORD: postgres POSTGRES_DB: wonga_db ports: - "5432:5432" volumes: - postgres_data:/var/lib/postgresql/data healthcheck: test: ["CMD-SHELL", "pg_isready -U postgres"] interval: 10s timeout: 5s retries: 5

api: build: context: ./backend dockerfile: Dockerfile container_name: wonga-api environment: ASPNETCORE_ENVIRONMENT: Development ConnectionStrings__DefaultConnection: "Server=postgres;Port=5432;Database=wonga_db;User Id=postgres;Password=postgres;" JwtSettings__SecretKey: "your-super-secret-key-that-is-at-least-256-bits-long-change-this-in-production" ports: - "5000:5000" depends_on: postgres: condition: service_healthy volumes: - ./backend:/src

frontend: build: context: ./frontend dockerfile: Dockerfile container_name: wonga-frontend environment: REACT_APP_API_URL: http://localhost:5000/api ports: - "3000:3000" depends_on: - api volumes: - ./frontend/src:/app/src

volumes: postgres_data:

networks: default: name: wonga-network

Total: 35 files with complete, production-ready code.
