const Login: React.FC = () => {
  const handleGoogleLogin = () => {
    window.location.href = "/auth/google-login";
  };

  return (
    <div className="flex items-center justify-center h-screen">
      <div className="bg-white shadow rounded p-6 w-80">
        <h2 className="text-2xl text-black font-bold mb-4 text-center">Login 🗫</h2>
        <button
          type="button"
          onClick={handleGoogleLogin}
          className="btn btn-outline text-neutral hover:text-white w-full flex items-center justify-center gap-2 mt-2"
        >
          <img
            src="https://upload.wikimedia.org/wikipedia/commons/c/c1/Google_%22G%22_logo.svg"
            alt="Google"
            className="w-5 h-5"
          />
          Continue with Google
        </button>
      </div>
    </div>
  );
};
export default Login;