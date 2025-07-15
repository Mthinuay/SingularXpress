import React, { useState, useEffect } from 'react';
import { Routes, Route, useNavigate, useLocation } from 'react-router-dom';
import SignIn from './Components/SignIn';
import ForgotPassword from './Components/password';
import AddEmployee from './Components/AddEmployee';
import EditEmployee from './Components/EditEmployee';
import MenuBar from './Components/MenuBar';
import TaxTableUpload from "./Components/TaxTableUpload";  // add this import
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './App.css';
import './MenuBar.css';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [showForgotPassword, setShowForgotPassword] = useState(false);
  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    const params = new URLSearchParams(window.location.search);
    const step = params.get('step');
    if (step === 'otp' || step === 'newPassword') {
      setShowForgotPassword(true);
    }
  }, []);

  const handleForgotPasswordClick = () => {
    setShowForgotPassword(true);
  };

  const handleBackToLogin = () => {
    setShowForgotPassword(false);
    navigate('/');
  };

  const handleLoginSuccess = () => {
    setIsLoggedIn(true);
    navigate('/dashboard');
  };

  if (!isLoggedIn) {
    return (
      <div className="App">
        <div className="circle-one"></div>
        <div className="circle-two"></div>
        <div className="circle-three"></div>
        {showForgotPassword ? (
          <>
            <div className="ellipse"></div>
            <ForgotPassword onBackToLogin={handleBackToLogin} />
          </>
        ) : (
          <SignIn
            onForgotPasswordClick={handleForgotPasswordClick}
            onLoginSuccess={handleLoginSuccess}
          />
        )}
      </div>
    );
  }

  return (
    <div className="App" style={{ display: 'flex', minHeight: '100vh' }}>
      <MenuBar />
      <div style={{ flex: 1, padding: '1rem' }}>
        <ToastContainer position="top-right" autoClose={3000} />
        <Routes>
          <Route path="/dashboard" element={<div>Welcome to Dashboard</div>} />
          <Route path="/addEmployee" element={<AddEmployee />} />
          <Route path="/editEmployee" element={<EditEmployee />} />
          <Route path="/taxTableUpload" element={<TaxTableUpload />} /> {/* Add route */}
        </Routes>
      </div>
    </div>
  );
}

export default App;
