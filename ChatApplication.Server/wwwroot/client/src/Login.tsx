import { useState } from "react";

const Login: React.FC = () => {
  const [username, setUsername] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [error, setError] = useState<string>("");
  const [isRegistering, setIsRegistering] = useState<boolean>(false);
  const [successMessage, setSuccessMessage] = useState<string>("");

  const handleLogin = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError("");
    setSuccessMessage("");
    try {
      const res = await fetch("/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password }),
      });

      if (!res.ok) {
        throw new Error("Wrong Username or Password");
      }

      const data = await res.json();
      sessionStorage.setItem("jwt", data.token);
      location.reload();
    } catch (err: unknown) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError("An unknown error occurred.");
      }
    }
  };

  const handleRegister = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError("");
    setSuccessMessage("");
    try {
      const res = await fetch("/api/auth/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password }),
      });

      if (!res.ok) {
        const errorData = await res.json();
        const errorMessage =
          errorData.errors?.[0]?.description || "Registration failed.";
        throw new Error(errorMessage);
      }

      setSuccessMessage("Registration successful! Please log in.");
      setIsRegistering(false);
      setUsername("");
      setPassword("");
    } catch (err: unknown) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError("An unknown error occurred during registration.");
      }
    }
  };

  const toggleForm = () => {
    setIsRegistering(!isRegistering);
    setError("");
    setSuccessMessage("");
    setUsername("");
    setPassword("");
  };

  return (
    <div className="flex items-center justify-center h-screen">
      <form
        onSubmit={isRegistering ? handleRegister : handleLogin}
        className="bg-white shadow rounded p-6 w-80"
      >
        <h2 className="text-2xl text-black font-bold mb-4">
          {isRegistering ? "Register" : "Login"} 🗫
        </h2>
        {error && <p className="text-red-500 font-semibold mb-2">{error}</p>}
        {successMessage && (
          <p className="text-green-500 font-semibold mb-2">
            {successMessage}
          </p>
        )}
        <input
          type="text"
          placeholder="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          className="input input-bordered w-full mb-2"
        />
        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="input input-bordered w-full mb-2"
        />
        <button type="submit" className="btn btn-primary w-full">
          {isRegistering ? "Register" : "Login"}
        </button>
        <div className="text-center mt-4">
          <button
            type="button"
            onClick={toggleForm}
            className="link text-sm"
          >
            {isRegistering ? (
              <p className="text-gray-900">Already have an account? Login</p>
            ) : (
              <p className="text-gray-900">Don't have an account? Register</p>
            )}
          </button>
        </div>
      </form>
    </div>
  );
};
export default Login;