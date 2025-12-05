const panel = document.getElementById("aiPanel");
const toggleBtn = document.getElementById("aiToggleBtn");
const input = document.getElementById("aiInput");
const sendBtn = document.getElementById("aiSendBtn");
const messages = document.getElementById("aiMessages");

toggleBtn.addEventListener("click", () => {
    if (panel.style.right === "0px") {
        panel.style.right = "-400px";
    } else {
        panel.style.right = "0px";
    }
});

sendBtn.addEventListener("click", async () => {
    const text = input.value.trim();
    if (!text) return;

    messages.innerHTML += `<div><b>You:</b> ${text}</div>`;
    input.value = "";

    const response = await fetch("/AI/Ask", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ message: text })
    });

    const data = await response.json();
    messages.innerHTML += `<div><b>AI:</b> ${data.reply}</div>`;
    messages.scrollTop = messages.scrollHeight;
});
