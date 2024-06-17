# 使用 Ubuntu 作為基礎映像
FROM ubuntu:20.04

# 安裝必要的依賴和工具，包括網絡調試工具
RUN apt-get update && apt-get install -y \
    libglib2.0-0 \
    libsm6 \
    libxrender1 \
    libxext6 \
    iproute2 \
    net-tools \
    curl \
    wget \
    telnet \
    traceroute \
    netcat \
    iputils-ping

# 複製構建結果到映像
COPY ./Builds/DedicatedServer /usr/local/server

# 設定工作目錄
WORKDIR /usr/local/server

# 暴露所需端口（例如 7777）
EXPOSE 7777

# 啟動伺服器
CMD ["./server.x86_64", "-batchmode", "-nographics", "-logfile", "/usr/local/server/server.log", "-address", "0.0.0.0", "-port", "7777"]
