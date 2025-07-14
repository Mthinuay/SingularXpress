import React, { useState } from 'react';
import "../../Navy.css";
import { useNavigate } from 'react-router-dom';

const SignIn = ({ onForgotPasswordClick, onLoginSuccess }) => {
  const [email, setEmail] = useState(''); 
  const [password, setPassword] = useState(''); 
  const [error, setError] = useState('');
  const [showPassword, setShowPassword] = useState(false); 
  const [attemptCount, setAttemptCount] = useState(0);
  const navigate = useNavigate(); 


  const togglePasswordVisibility = () => {
    
    setShowPassword(!showPassword); 
  };

    const handleLogin = async () => {
      setError(''); 

    
      if (!email.trim()) {
        setError('Email is required.');
        return;
      }
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!emailRegex.test(email)) {
        setError('Please enter a valid email address.');
        return;
      }

      if (!password) {
        setError('Password is required.');
        return;
      }
      try {
        const response = await fetch('http://localhost:5037/api/user/login', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ email, password }),
        });

        const responseText = await response.text(); 

        if (response.ok) {
          console.log('Login successful:', responseText);
          setError('');
          setAttemptCount(0);
          if (typeof onLoginSuccess === 'function') {
            onLoginSuccess();
          }
        } else {
          setError(responseText);
          
          if (response.status === 401 && responseText.includes("Invalid email or password")) {
            setAttemptCount(prev => prev + 1);
          }
        }
      } catch (err) {
        console.error('Login error:', err);
        setError('Network error. Please check your connection.');
      }
    };
      const handleForgotPassword = () => {
    navigate('/reset-password'); 
  };

  return (
    <div className="signin-container">
      <div className="logo-container">
        <span className="logo-bold">singular</span>
        <span className="logo-light">express</span>
      </div>

      <div className="auth-content">
        <div className="column left-column">
          <div className="left-inner-column" style={{ marginLeft: '50px' }}>
            <div className="adjusted-content">
              <div className="welcome-text">Welcome!</div>
              <div className="log-details">
                <div className="input-group">
                  <img src="/images/mail2.svg" alt="email icon" className="input-icon-mail" />
                  <input
                    type="email"
                    placeholder="Enter your email"
                    className="input-field"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)} 
                  />
                </div>
                <div className="input-group">
                  <img src="/images/key2.svg" alt="password icon" className="input-icon-key" />
                  <input
                    type={showPassword ? 'text' : 'password'}
                    placeholder="Enter your password"
                    className="input-field"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)} 
                  />
                  <img
                    src="/images/visibility_off.svg"
                    alt="toggle visibility"
                    className="visibility-icon"
                    onClick={togglePasswordVisibility}
                    style={{ cursor: 'pointer' }}
                  />
                </div>
                {error && <div className="error-message">{error}</div>}
                <button className="sign-in-button" onClick={handleLogin}>
                  Sign in
                </button>
                <div
                  className="forgot-password"
                  onClick={onForgotPasswordClick} 
                  style={{ cursor: 'pointer' }}
                >
                  Forgot password
                </div>
                <div className="footer-text">
                  Privacy Policy | Terms & Conditions
                  <br />
                  Copyright © 2025 Singular Systems. All rights reserved.
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className="column right-column">
          <img
            src="/images/iMAGEgENFORWEB.svg"
            alt="Image Gen For Web"
            className="right-column-image"
          />
        </div>
      </div>
    </div>
  );
};

export default SignIn;