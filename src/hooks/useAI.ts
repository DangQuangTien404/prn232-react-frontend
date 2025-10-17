import { useMutation } from "@tanstack/react-query";
import { kyAspDotnet } from "src/services/ApiService";

export function useAiChatMutation() {
  return useMutation<string, Error, string>({
    mutationFn: async (message: string) => {
      try {
        const textResponse = await kyAspDotnet
          .post("api/ai/chat", {
            json: { message },
            timeout: false,
          })
          .text();

        const cleaned = textResponse
          .replace(/\\u0000-\\u001F/g, " ")
          .trim();

        const parsed = JSON.parse(cleaned);

        if (parsed && typeof parsed.response === "string") {
          return parsed.response;
        }

        return JSON.stringify(parsed);
      } catch (err) {
        console.error("🔥 Lỗi parse phản hồi AI:", err);
        return "⚠️ Lỗi khi đọc phản hồi từ AI server.";
      }
    },
  });
}
