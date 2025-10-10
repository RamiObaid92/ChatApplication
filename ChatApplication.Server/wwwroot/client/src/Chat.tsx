import { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import DOMPurify from "dompurify";

// Defines the structure for a message.
interface Message {
  user: string;
  content: string;
}

const Chat: React.FC = () => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [messages, setMessages] = useState<Message[]>([]);
  
  const [username, setUsername] = useState<string>("");
  const [newMessage, setNewMessage] = useState<string>("");

  const messagesEndRef = useRef<HTMLDivElement>(null);

  // Sets up the SignalR connection.
  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7277/chathub")
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, []);

  // Starts and stops the SignalR connection.
  useEffect(() => {
    if (connection) {
      connection.start()
        .then(() => console.log("SignalR Connected."))
        .catch(err => console.error("Connection failed: ", err));
      
      // Listens for new messages from the server.
      connection.on("ReceiveMessage", (user: string, message: string) => {
        const safeUser = DOMPurify.sanitize(user);
        const safeContent = DOMPurify.sanitize(message);
        setMessages(prev => [...prev, { user: safeUser, content: safeContent }]);
      });

      return () => {
        connection.stop();
      };
    }
  }, [connection]);

  // Scrolls to the bottom of the chat window when a new message is added.
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  // Sends a message to the server.
  const sendMessage = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (connection && username.trim() && newMessage.trim()) {
      try {
        await connection.invoke("SendMessage", username, newMessage);
        setNewMessage("");
      } catch (err) {
        console.error("Send message error:", err);
      }
    }
  };
  
  const canChat = username.trim().length > 0;

  return (
    <div className="p-6 max-w-lg mx-auto mt-10">
      <div className="h-96 overflow-y-scroll border rounded-lg p-4 bg-primary">
        {messages.map((msg, index) => (
          <div
            key={index}
            className={`chat ${msg.user === username ? "chat-end" : "chat-start"}`}
          >
            <div className="chat-header">{msg.user}</div>
            <div className={`chat-bubble ${msg.user === username ? "bg-gray-700" : ""}`}>
              {msg.content}
            </div>
          </div>
        ))}
        <div ref={messagesEndRef} />
      </div>

      <form onSubmit={sendMessage} className="mt-4 flex gap-2">
        <input
          type="text"
          placeholder="Ditt namn..."
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          className="input input-bordered w-1/3"
        />
        <input
          type="text"
          placeholder={canChat ? "Skriv ett meddelande..." : "Ange ett namn för att chatta"}
          value={newMessage}
          onChange={(e) => setNewMessage(e.target.value)}
          className="input input-bordered flex-1"
          disabled={!canChat}
        />
        <button type="submit" className="btn btn-primary" disabled={!canChat}>
          Skicka
        </button>
      </form>
    </div>
  );
};

export default Chat;