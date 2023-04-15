from langchain.agents import ZeroShotAgent, AgentExecutor, load_tools
from langchain.llms import OpenAIChat
from langchain import LLMChain

def setup_agent():
    llm = OpenAIChat(temperature=0)
    tools = load_tools(["google-search"], llm=llm)
    prefix = """次の質問にできる限り答えてください。次のツールにアクセスできます:"""
    suffix = """始めましょう! 最終的な答えを出すときは、一人称は"ぼく"、語尾には"なのだ"を使用してください

    Question: {input}
    {agent_scratchpad}"""

    prompt = ZeroShotAgent.create_prompt(
        tools,
        prefix=prefix,
        suffix=suffix,
        input_variables=["input", "agent_scratchpad"]
    )

    llm_chain = LLMChain(llm=llm, prompt=prompt)
    agent = ZeroShotAgent(llm_chain=llm_chain, tools=tools)
    agent_executor = AgentExecutor.from_agent_and_tools(agent=agent, tools=tools, verbose=True)

    return agent_executor
