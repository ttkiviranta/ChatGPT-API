General Idea of this demo:

1. Divide your long text into small chunks that are consumable by GPT
2. Store each chunk in the vector database. Each chunk is indexed by a chunk embedding vector
3. When asking a question to GPT, convert the question to a question embedding vector first
4. Use question embedding vector to search chunks from vector database
5. Combine chunks and questions as a prompt, feed it to GPT
6. Get the response from GPT

Design:
![image](https://github.com/ttkiviranta/ChatGPT-API/assets/22675135/23689099-69e2-4e7e-b13c-6bb711778bb7)

As described in the above section, actors can do two things:

- Upload external data to the database
- Ask questions about the external data

So two actors are present in the diagram.

We use two OpenAI models in this demo:

- gpt-3.5-turbo 
- text-embedding-ada-002 (for generating accurate embedding vectors) 

Query Interface:

- It takes the user’s input question and calls text-embedding-ada-002 to generate an embedding. With this embedding vector, it then queries the Database Interface to search for chunks related to the questions
- It combines retrieved chunks and the user’s original question and query gpt-3.5-turbo (ChatGPT)
- Then it returns ChatGPT’s answer back to users.

Database Interface:

- Search: it takes the user’s question from Query Interface and queries the vector database
- Insert: it takes long raw text from the user, chunks the text into small pieces, converts each piece into an embedding vector, and inserts the <embedding_vector, chunk> pairs into the database.

Vector Database:

- The actual database behind the database interface. In this demo, it is MS SQL Database


Database diagrams:

![image](https://github.com/ttkiviranta/ChatGPT-API/assets/22675135/3b10ed74-6a74-4cd6-b39f-6168baa8d7ab)
