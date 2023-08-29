
list='2.10/notes.txt
2.11/notes.txt
2.12/notes.txt
2.13/notes.txt
2.14/notes.txt
2.15/notes.txt
2.16/notes.txt
2.18/notes.txt
2.19/notes.txt
2.20/notes.txt
2.21/notes.txt
2.22/notes.txt
2.23/notes.txt
2.24/notes.txt
2.25/notes.txt
2.26/notes.txt
2.27/notes.txt
2.28/notes.txt
2.29/notes.txt
2.3/notes.txt
2.30/notes.txt
2.31/notes.txt
2.32/notes.txt
2.33/notes.txt
2.34/notes.txt
2.35/notes.txt
2.36/notes.txt
2.37/notes.txt
2.38/notes.txt
2.39/notes.txt
2.4/notes.txt
2.40/notes.txt
2.41/notes.txt
2.42/notes.txt
2.43/notes.txt
2.44/notes.txt
2.45/notes.txt
2.46/notes.txt
2.5/notes.txt
2.6/notes.txt
2.7/notes.txt
2.8/notes.txt
2.9/notes.txt'
for v in $list; do
  name=$(dirname $v); mkdir -p legacy-notes/$name; echo $v: [$name]
  curl -kSL -o legacy-notes/$name-notes.txt https://chromedriver.storage.googleapis.com/$v
done
