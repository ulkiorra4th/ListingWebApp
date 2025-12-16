FROM node:18-alpine AS deps
WORKDIR /app
COPY package.json package-lock.json* ./
RUN npm install --legacy-peer-deps || (\
  echo "Falling back to install without lockfile" && rm -f package-lock.json && npm install --legacy-peer-deps)

FROM node:18-alpine AS app
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY . .
EXPOSE 5173
CMD ["npm", "run", "dev"]
