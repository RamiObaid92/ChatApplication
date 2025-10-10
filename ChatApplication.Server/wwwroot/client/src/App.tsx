import { useState, useEffect } from "react";
import Login from "./Login";
import Chat from "./Chat";

const App: React.FC = () => {
  const [jwt, setJwt] = useState<string | null>(() =>
    sessionStorage.getItem("jwt")
  );

  useEffect(() => {
    const params = new URLSearchParams(window.location.search);
    const token = params.get("token");
    if (token) {
      sessionStorage.setItem("jwt", token);
      setJwt(token);
      window.history.replaceState({}, document.title, window.location.pathname);
    }
  }, []);

  const handleLogout = () => {
    sessionStorage.removeItem("jwt");
    setJwt(null);
  };

  return (
    <>
      <nav className="bg-primary text-white h-12 flex items-center justify-between px-4">
        {jwt && (
          <button
            onClick={handleLogout}
            className="bg-white text-primary px-2 py-1 rounded text-sm cursor-pointer"
          >
            Logout
          </button>
        )}
      </nav>

      {jwt ? <Chat /> : <Login />}
    </>
  );
};
export default App;