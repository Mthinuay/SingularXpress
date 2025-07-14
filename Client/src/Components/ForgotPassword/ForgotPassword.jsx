import React, { useState, useEffect } from 'react';
import "../../Navy.css";
import { requestResetLink, verifyOtp, updatePassword } from '../Services/authServices';
import RequestResetForm from './RequestResetForm';
import OtpVerificationForm from './OtpVerificationForm';
import NewPasswordForm from './NewPasswordForm';

const ForgotPassword = ({ onBackToLogin }) => {
  const [currentStep, setCurrentStep] = useState('request');
  const [email, setEmail] = useState('');
  const [otp, setOtp] = useState(Array(4).fill(''));
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  useEffect(() => {
    const queryParams = new URLSearchParams(window.location.search);
    const emailParam = queryParams.get('email');
    const stepParam = queryParams.get('step');

    if (emailParam) setEmail(emailParam);
    if (stepParam === 'otp') {
      setCurrentStep('otp');
    }
  }, []);

  const handleRequestResetLink = async () => {
    setIsLoading(true);
    setError('');
    try {
      await requestResetLink(email);
      setCurrentStep('otp');
    } catch (err) {
      setError(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  const handleVerifyOtp = async () => {
    setIsLoading(true);
    setError('');
    try {
      const isValid = await verifyOtp(email, otp.join(''));
      if (isValid.valid) {
        setCurrentStep('newPassword');
      } else {
        setError('Invalid OTP. Please try again.');
      }
    } catch (err) {
      console.error('Error verifying OTP:', err);
      setError('Invalid OTP. Please enter the code sent to your email and try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleUpdatePassword = async () => {
    if (newPassword !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }

    try {
      await updatePassword(email, newPassword);
      setSuccessMessage('Password updated successfully!');
      setError('');
    } catch (err) {
      console.error('Error updating password:', err);
      setError(err.response?.data || 'Failed to update password. Please ensure your new passwords match and try again.');
    }
  };

  const handleOtpChange = (index, value) => {
    const newOtp = [...otp];
    newOtp[index] = value;
    setOtp(newOtp);

    if (value && index < 3) {
      document.getElementById(`otp-input-${index + 1}`).focus();
    }
  };

  const handleKeyDown = (index, event) => {
    if (event.key === 'Backspace' && !otp[index] && index > 0) {
      document.getElementById(`otp-input-${index - 1}`).focus();
    }
  };

  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  const renderStep = () => {
    switch (currentStep) {
      case 'otp':
        return (
          <OtpVerificationForm
            otp={otp}
            handleOtpChange={handleOtpChange}
            handleKeyDown={handleKeyDown}
            onSubmit={handleVerifyOtp}
            isLoading={isLoading}
            error={error}
            onBackToLogin={onBackToLogin}
          />
        );
      case 'newPassword':
        return (
          <NewPasswordForm
            newPassword={newPassword}
            setNewPassword={setNewPassword}
            confirmPassword={confirmPassword}
            setConfirmPassword={setConfirmPassword}
            showPassword={showPassword}
            togglePasswordVisibility={togglePasswordVisibility}
            onSubmit={handleUpdatePassword}
            error={error}
            successMessage={successMessage}
          />
        );
      default:
        return (
          <RequestResetForm
            email={email}
            setEmail={setEmail}
            onSubmit={handleRequestResetLink}
            isLoading={isLoading}
            error={error}
            onBackToLogin={onBackToLogin}
          />
        );
    }
  };

  return (
    <div className="signin-container">
      <div className="logo-container">
        <span className="logo-bold">singular</span>
        <span className="logo-light">express</span>
      </div>

      <div className="auth-content">
        <div className="column left-column">
          <img
            src="/images/password_image.png"
            alt="Reset Password"
            className="password-image"
          />
        </div>

        <div className="column right-column">
          {renderStep()}
          
          {currentStep !== 'newPassword' && (
            <div className="footer-text">
              Privacy Policy | Terms & Conditions
              <br />
              Copyright © 2025 Singular Systems. All rights reserved.
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ForgotPassword;