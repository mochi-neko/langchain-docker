from langchain.llms import OpenAI

# LLMの準備
llm = OpenAI(temperature=0.9)

# LLMの呼び出し
print(llm("コンピュータゲームを作る日本語の新会社名をを1つ提案してください"))