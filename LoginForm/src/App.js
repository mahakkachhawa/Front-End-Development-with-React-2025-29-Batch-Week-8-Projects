import React, { useState } from "react";

// Returns "Weak", "Medium" or "Strong" (or null when the password is empty)
function getPasswordStrength(password) {
  if (password.length === 0) return null;
  if (password.length < 6) return "Weak";
  if (password.length < 10) return "Medium";
  return "Strong";
}

function App() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [emailError, setEmailError] = useState("");
  const [passwordError, setPasswordError] = useState("");
  const [successMsg, setSuccessMsg] = useState("");

  // Typing in the email field clears the email error and the success message
  const handleEmailChange = (event) => {
    setEmail(event.target.value);
    setEmailError("");
    setSuccessMsg("");
  };

  // Typing in the password field clears the password error and the success message
  const handlePasswordChange = (event) => {
    setPassword(event.target.value);
    setPasswordError("");
    setSuccessMsg("");
  };

  const handleTogglePassword = () => {
    setShowPassword((prev) => !prev);
  };

  const handleLogin = () => {
    const trimmedEmail = email.trim();
    const isEmailValid = trimmedEmail.includes("@");
    const isPasswordValid = password.length >= 6;

    // Each field is validated separately, so both errors can show together
    setEmailError(isEmailValid ? "" : "Enter a valid email");
    setPasswordError(isPasswordValid ? "" : "Password must be at least 6 characters");

    if (isEmailValid && isPasswordValid) {
      setSuccessMsg(`Login successful for ${trimmedEmail}`);
    } else {
      setSuccessMsg("");
    }
  };

  const strength = getPasswordStrength(password);

  return (
    <div className="page">
      <div className="login-card">
        <h1 id="title" className="login-title">Login</h1>

        <label className="field-label" htmlFor="emailInput">Email</label>
        <input
          id="emailInput"
          className={emailError ? "text-input input-error" : "text-input"}
          type="email"
          placeholder="you@example.com"
          value={email}
          onChange={handleEmailChange}
        />
        {emailError && <p id="emailError" className="error-text">{emailError}</p>}

        <label className="field-label" htmlFor="passwordInput">Password</label>
        <div className="password-row">
          <input
            id="passwordInput"
            className={passwordError ? "text-input input-error" : "text-input"}
            type={showPassword ? "text" : "password"}
            placeholder="Enter your password"
            value={password}
            onChange={handlePasswordChange}
          />
          <button id="togglePassword" className="toggle-btn" onClick={handleTogglePassword}>
            {showPassword ? "Hide" : "Show"}
          </button>
        </div>

        {strength && (
          <p id="passwordStrength" className={`strength strength-${strength.toLowerCase()}`}>
            {strength}
          </p>
        )}
        {passwordError && <p id="passwordError" className="error-text">{passwordError}</p>}

        <button id="loginBtn" className="login-btn" onClick={handleLogin}>
          Login
        </button>

        {successMsg && <p id="successMsg" className="success-msg">{successMsg}</p>}
      </div>
    </div>
  );
}

export default App;