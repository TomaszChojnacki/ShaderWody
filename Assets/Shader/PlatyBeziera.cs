using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BezierWaterMesh : MonoBehaviour
{
    public int resolution = 10; // Punktow na osi plata (rozdzielczosc siatki)
    private Mesh mesh;

    void Start()
    {
        // Tworzymy nowa siatke i przypisujemy ja do komponentu MeshFilter
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Generowanie siatki na podstawie funkcji Bezier
        GenerateMesh();
    }

    // Funkcja pomocnicza — funkcja Béziera dla 4 punktów
    Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t; // Ustalanie wspolczynnika odwrotnego
        // Obliczanie punktu na krzywej Béziera w zaleznosci od t
        return u * u * u * p0 + 3 * u * u * t * p1 + 3 * u * t * t * p2 + t * t * t * p3;
    }

    // Funkcja generujaca siatkê na podstawie 4 platow Béziera (2x2)
    void GenerateMesh()
    {
        mesh.Clear(); // Czyscimy siatke przed generowaniem nowych danych

        int count = resolution + 1; // Liczba wierzcholkow w jednej osi (rozmiar siatki)
        Vector3[] vertices = new Vector3[count * count]; // Tablica wierzcholkow
        Vector2[] uv = new Vector2[count * count]; // Tablica wspolrzednych UV (do tekstur)
        int[] triangles = new int[resolution * resolution * 6]; // Tablica trojkatow (6 indeksow na 2 trojkaty)

        // Tworzymy kontrolne punkty dla siatki (7x7), ktore beda uzywane do obliczen Béziera
        Vector3[,] controlPoints = new Vector3[7, 7];

        // Generowanie wysokosci kontrolnych punktow za pomoca Perlin Noise
        for (int y = 0; y < 7; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                // Generowanie wysokosci za pomoca PerlinNoise, aby stworzyc naturalne wzory
                float height = Mathf.PerlinNoise(x * 0.3f, y * 0.3f) * 0.2f; // Zmniejszenie wplywu wysokosci
                controlPoints[x, y] = new Vector3(x, height, y); // Przypisanie kontrolnych punktow
            }
        }

        // Generowanie wierzcholkow na siatce
        for (int y = 0; y <= resolution; y++)
        {
            float v = y / (float)resolution; // Normalizowanie wspolrzednej Y (pionowo)
            float fy = v * 2f; // Rozciaganie na dwa platy w pionie
            int patchY = Mathf.Min((int)fy, 1); // Wybor p³ata w pionie (0 lub 1)
            float ty = fy - patchY; // Normalizacja wspolrzednej t w pionie

            for (int x = 0; x <= resolution; x++)
            {
                float u = x / (float)resolution; // Normalizowanie wspolrzednej X (poziomo)
                float fx = u * 2f; // Rozciaganie na dwa platy w poziomie
                int patchX = Mathf.Min((int)fx, 1); // Wybor plata w poziomie (0 lub 1)
                float tx = fx - patchX; // Normalizacja wspolrzednej t w poziomie

                // Pobieramy kontrolne punkty dla danego plata (4x4)
                Vector3[] row = new Vector3[4]; // Tablica do przechowywania punktow na jednym wierszu plata
                for (int i = 0; i < 4; i++)
                {
                    row[i] = Bezier(
                        controlPoints[patchX * 3 + 0, patchY * 3 + i],
                        controlPoints[patchX * 3 + 1, patchY * 3 + i],
                        controlPoints[patchX * 3 + 2, patchY * 3 + i],
                        controlPoints[patchX * 3 + 3, patchY * 3 + i],
                        tx); // Obliczamy punkt na podstawie funkcji Béziera w poziomie
                }

                // Obliczamy ostateczny punkt 3D na siatce
                Vector3 finalPoint = Bezier(row[0], row[1], row[2], row[3], ty); // Obliczamy punkt na podstawie funkcji Béziera w pionie
                int index = y * count + x; // Obliczamy indeks wierzcho³ka w tablicy
                vertices[index] = finalPoint; // Przypisujemy punkt do wierzcholkow
                uv[index] = new Vector2(u, v); // Przypisujemy wspolrzedne UV
            }
        }

        // Generowanie trojkatow (indeksowanie)
        int t = 0; // Licznik trojkatosw
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = y * count + x; // Indeks wierzcholka
                triangles[t++] = i; // Pierwszy wierzcholek
                triangles[t++] = i + count; // Drugi wierzcholek
                triangles[t++] = i + 1; // Trzeci wierzcholek

                triangles[t++] = i + 1; // Pierwszy wierzcholek drugiego trojkata
                triangles[t++] = i + count; // Drugi wierzcholek drugiego trojkata
                triangles[t++] = i + count + 1; // Trzeci wierzcholek drugiego trojkata
            }
        }

        // Przypisanie obliczonych wierzcholkow, trojkatow i wspolrzednych UV do siatki
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // Ponowne obliczenie normalnych
    }

    // Funkcja do animowania falowania na siatce
    void Update()
    {
        Vector3[] verts = mesh.vertices; // Pobieramy wierzcholki siatki
        for (int i = 0; i < verts.Length; i++)
        {
            // Modyfikujemy wysokosc wierzcholkow na podstawie funkcji sinusoidalnej (falowanie)
            verts[i].y += Mathf.Sin(Time.time * 2f + verts[i].x + verts[i].z) * 0.01f;
        }
        mesh.vertices = verts; // Zapisujemy zmodyfikowane wierzcholki
        mesh.RecalculateNormals(); // Ponowne obliczenie normalnych
    }
}
